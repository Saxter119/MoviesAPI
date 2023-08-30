using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System;

namespace MoviesAPI.Helpers
{
    public class MovieExistAtributte : Attribute, IAsyncResourceFilter
    {
        private readonly ApplicationDbContext dbContext;

        public MovieExistAtributte(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var movieIdObject = context.HttpContext.Request.RouteValues["movieId"];

            if (movieIdObject == null) { return; }

            var movieId = int.Parse(movieIdObject.ToString());

            var movie = await dbContext.Movies.AnyAsync(x => x.Id == movieId);

            if (!movie) context.Result = new NotFoundResult();

            else await next();
        }
    }
}
