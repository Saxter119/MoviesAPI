using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;

namespace MoviesAPI.Helpers
{
    public static class HttpContextExtensions
    {
        public async static Task CountTotalOfRecords<T>(this HttpContext httpContext, IQueryable<T> queryable, int recordForPage)
        {
            double sum = await queryable.CountAsync();
            double totalPages = Math.Ceiling(sum / recordForPage);
            httpContext.Response.Headers.Add("TotalPages", totalPages.ToString());
        }
    }
}
