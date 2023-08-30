using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.DTOs.RoomMovies;
using MoviesAPI.Entities;
using NetTopologySuite.Geometries;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("api/roommovies")]
    public class RoomMoviesController : CustomBaseController
    {
        private readonly ApplicationDbContext appDbContext;
        private readonly GeometryFactory geometryFactory;

        public RoomMoviesController(ApplicationDbContext appDbContext, IMapper mapper, GeometryFactory geometryFactory) 
            : base(appDbContext, mapper)
        {
            this.appDbContext = appDbContext;
            this.geometryFactory = geometryFactory;
        }
        [HttpGet]
        public async Task<List<RoomMovieDTO>> Get()
        {
            return await Get<RoomMovie, RoomMovieDTO>();
        }
        [HttpGet(("{id:int}"), Name = "getRoomMovieById")]
        public async Task<ActionResult<RoomMovieDTO>> Get(int id)
        {
            return await Get<RoomMovie, RoomMovieDTO>(id);
        }
        [HttpGet("near")]
        public async Task<ActionResult<List<RoomMovieNearDTO>>> Get([FromQuery] RoomMovieNearFilterDTO filter)
        {
            var userLocation = geometryFactory.CreatePoint(new Coordinate() { Y = filter.Latitude, X = filter.Longitude });

            var roomMovies = await appDbContext.RoomMovies.Where(x => x.Location.IsWithinDistance(userLocation, filter.DistanceOnKms
                * 1000)).OrderBy(x => x.Location.Distance(userLocation))
                .Select(x => new RoomMovieNearDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                    Latitude = x.Location.Y,
                    Longitude = x.Location.X,
                    DistanceOnMeter = Math.Round(x.Location.Distance(userLocation))
                }).ToListAsync(); 

            return roomMovies;
        }
        [HttpPost]
        public async Task<ActionResult> Post([FromBody]RoomMovieCreationDTO roomMovieCreationDTO)
        {
            return await Post<RoomMovie, RoomMovieCreationDTO, RoomMovieDTO>(roomMovieCreationDTO, "getRoomMovieById");
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put([FromBody]RoomMovieCreationDTO roomMovieCreationDTO, int id)
        {
            return await Put<RoomMovie, RoomMovieCreationDTO>(roomMovieCreationDTO, id);
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<RoomMovie>(id);
        }

    }
}
