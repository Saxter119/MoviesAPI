using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.DTOs.ReviewsDTOs;
using MoviesAPI.Entities;
using MoviesAPI.Helpers;
using System.Security.Claims;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("api/movies/{movieid:int}/reviews")]
    [ServiceFilter(typeof(MovieExistAtributte))]
    public class ReviewsController : CustomBaseController
    {
        private readonly ApplicationDbContext appContext;
        private readonly IMapper mapper;

        public ReviewsController(ApplicationDbContext appContext, IMapper mapper) : base(appContext, mapper)
        {
            this.appContext = appContext;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<ReviewDTO>>> Get(int movieId, [FromQuery] PaginationDTO paginationDTO)
        {
            var query = appContext.Reviews.Where(x => x.MovieId == movieId).Include(x => x.User).AsQueryable();
            return await Get<Review, ReviewDTO>(paginationDTO, query);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<ActionResult> Post(int movieId, ReviewCreationDTO reviewCreationDTO)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            var existReview = await appContext.Reviews.AnyAsync(x => x.Id == movieId && x.UserId == userId);

            if (existReview) return Conflict("You have already created a review on this movie");

            var review = mapper.Map<Review>(reviewCreationDTO);

            review.MovieId = movieId;
            review.UserId = userId;

            appContext.Add(review);
            await appContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{reviewId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Put( int reviewId, ReviewCreationDTO newReview)
        {
           
            var review = await appContext.Reviews.FirstOrDefaultAsync(x => x.Id == reviewId);

            if (review == null) return NotFound($"There is not any review with the id: { reviewId} on this movie");

            var user = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            if (user != review.UserId) return BadRequest("You don't the permision to modify this comment");

            mapper.Map(newReview, review);

            await appContext.SaveChangesAsync();

            return NoContent();
        }
        [Authorize(JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("{reviewId}")]
        public async Task<ActionResult> Delete(int movieId, int reviewId)
        {
            var review = await appContext.Reviews.FirstOrDefaultAsync(x => x.MovieId == movieId && x.Id == reviewId);

            if (review is null) return NotFound($"There is not any review with the id: { reviewId} on this movie");

            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            if (userId != review.UserId) return BadRequest("You don't the permision to modify this comment");

            appContext.Remove(review);

            await appContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
