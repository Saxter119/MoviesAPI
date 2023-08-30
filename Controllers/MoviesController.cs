using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Helpers;
using MoviesAPI.Services;
using System.Linq.Dynamic.Core;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("api/movies")]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IFileStorage fileStorage;
        private readonly ILogger logger;
        private readonly string container = "movies";

        public MoviesController(ApplicationDbContext appDbContext, IMapper mapper, IFileStorage fileStorage, ILogger<MoviesController> logger)
        {
            this.dbContext = appDbContext;
            this.mapper = mapper;
            this.fileStorage = fileStorage;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<MovieIndexDTO> Get()
        {
            int top = 5;
            DateTime today = DateTime.Today;

            var newMovies = await dbContext.Movies.Where(movie => movie.OnCinemas).OrderBy(x => x.DateRelease).Take(top).ToListAsync();

            var nextMovies = await dbContext.Movies.Where(movie => movie.DateRelease > today)
                .OrderBy(x => x.DateRelease).Take(5).ToArrayAsync();

            var moviesIndex = new MovieIndexDTO();

            moviesIndex.NewMovies = mapper.Map<List<MovieDTO>>(newMovies);

            moviesIndex.NextMovies = mapper.Map<List<MovieDTO>>(nextMovies);

            return moviesIndex;
        }

        [HttpGet("{id}", Name = "getById")]
        public async Task<ActionResult<MovieWithDetailsDTO>> Get([FromRoute] int id)
        {
            var entity = await dbContext.Movies
                .Include(x=> x.MoviesActors).ThenInclude(x=> x.Actor)
                .Include(x=> x.MoviesGenders).ThenInclude(x=> x.Gender).FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null) return NotFound("You just got another 404 lol");

            entity.MoviesActors = entity.MoviesActors.OrderBy(x => x.Order).ToList();

            return mapper.Map<MovieWithDetailsDTO>(entity);
        }

        [HttpGet("filter")]
        public async Task<ActionResult<List<MovieDTO>>> FilterMovies([FromQuery]MovieFilterDTO movieFilterDTO)
        {
            var moviesQueryable = dbContext.Movies.AsQueryable();

            if(!string.IsNullOrEmpty(movieFilterDTO.Name))
            {
                moviesQueryable = moviesQueryable.Where(x => x.Name.Contains(movieFilterDTO.Name));
            }

            if(movieFilterDTO.OnCinemas)
            {
                moviesQueryable = moviesQueryable.Where(x => x.OnCinemas);
            }

            if (movieFilterDTO.NextMovies)
            {
                moviesQueryable = moviesQueryable.Where(x => x.DateRelease > DateTime.Today);
            }

            if(movieFilterDTO.GenderId != 0)
            {
                moviesQueryable = moviesQueryable.Where(x => x.MoviesGenders.Select(x => x.GenderId).Contains(movieFilterDTO.GenderId));
            }

            await HttpContext.CountTotalOfRecords(moviesQueryable, movieFilterDTO.PageRecords); //cuando llamas a un metodo estatico al cual le agregaste una extesion debes de llamar al metodo original (el que utiliza this) y no al metodo de extension

            if(!string.IsNullOrEmpty(movieFilterDTO.OrderField))
            {   
                string OrderAsc = movieFilterDTO.OrderAsc ? "ascending" : "descending";

                try
                {
                    moviesQueryable = moviesQueryable.OrderBy($"{movieFilterDTO.OrderField} {OrderAsc}");
                }
                catch (Exception ex)
                {
                    logger.Log(LogLevel.Error, ex.ToString(), ex);
                    //throw new Exception(ex.ToString());
                }
            }

            List<Movie> movies = await moviesQueryable.Paginate(movieFilterDTO.Paginate).ToListAsync();

            return mapper.Map<List<MovieDTO>>(movies);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] MovieCreationDTO movieCreationDTO)
        {
            var movie = mapper.Map<Movie>(movieCreationDTO);

            if (movieCreationDTO.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await movieCreationDTO.Poster.CopyToAsync(memoryStream);

                    var photoContent = memoryStream.ToArray();

                    var extension = Path.GetExtension(movieCreationDTO.Poster.FileName);

                    movie.Poster = await fileStorage.SaveFile(photoContent, extension, container, movieCreationDTO.Poster.ContentType);
                }
            }

            AsignOrder(movie);

            dbContext.Add(movie);

            await dbContext.SaveChangesAsync();

            var movieDTO = mapper.Map<MovieDTO>(movie);

            return CreatedAtRoute("getById", new { id = movieDTO.Id }, movieDTO);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put([FromRoute] int id, [FromForm] MovieCreationDTO movieCreationDTO)
        {
            var movie = await dbContext.Movies.Include(x => x.MoviesActors).Include(y => y.MoviesGenders)
                .FirstOrDefaultAsync(x => x.Id == id);//debemos hacer el include para incluir
                                                      //estos campos que representan las llaves foraneas

            if (movie == null) return NotFound();

            movie = mapper.Map(movieCreationDTO, movie);

            if (movieCreationDTO.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await movieCreationDTO.Poster.CopyToAsync(memoryStream);

                    var photoContent = memoryStream.ToArray();

                    var extension = Path.GetExtension(movieCreationDTO.Poster.FileName);

                    movie.Poster = await fileStorage.EditFile(photoContent, extension, container, movie.Poster, movieCreationDTO.Poster.ContentType);
                }
            }

            AsignOrder(movie);

            await dbContext.SaveChangesAsync();

            return NoContent();

        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch([FromRoute] int id, JsonPatchDocument<MoviePatchDTO> moviePatchDTO)
        {
            if (moviePatchDTO is null) return BadRequest();

            var entity = await dbContext.Movies.FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null) return NotFound();

            var patch = mapper.Map<MoviePatchDTO>(entity);

            moviePatchDTO.ApplyTo(patch, ModelState);

            var isValid = TryValidateModel(patch);

            if (!isValid) return BadRequest(ModelState);

            mapper.Map(patch, entity);

            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await dbContext.Movies.AnyAsync(x => x.Id == id);

            if (!exist) return NotFound($"Doesn't exist a movie with this id: {id}");

            dbContext.Remove(new Movie { Id = id });

            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        private static void AsignOrder(Movie movie)
        {

            if (movie.MoviesActors != null)
            {
                //for (int i = 0; i < movie.MoviesActors.Count; i++)
                //{
                //    movie.MoviesActors[i].Order = i;
                //}

                int i = 0;
                foreach (var actor in movie.MoviesActors)
                {
                    actor.Order = i;
                    i++;
                }
            }
        }
    }
}
