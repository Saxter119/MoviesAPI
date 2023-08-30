using AutoMapper;
using Microsoft.AspNetCore.Identity;
using MoviesAPI.DTOs;
using MoviesAPI.DTOs.AccountsDTOs;
using MoviesAPI.DTOs.ReviewsDTOs;
using MoviesAPI.Entities;
using NetTopologySuite.Geometries;

namespace MoviesAPI.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile(GeometryFactory geometryFactory)
        {
            //Users
            CreateMap<IdentityUser, UserDTO>();

            //Genders
            CreateMap<Gender, GenderDTO>().ReverseMap();
            CreateMap<GenderCreationDTO, Gender>();

            //Actors
            CreateMap<Actor, ActorDTO>().ReverseMap();
            CreateMap<ActorCreationDTO, Actor>().ForMember(x=> x.Photo, options=> options.Ignore());
            CreateMap<ActorPatchDTO, Actor>().ReverseMap();

            //Movies
            CreateMap<Movie, MovieDTO>().ReverseMap();
            CreateMap<Movie, MovieWithDetailsDTO>().ForMember(x=> x.GenderDTOs, option=> option.MapFrom(MapToMovieWithDetailsDTO))
                .ForMember(x=> x.ActorsDTOs, option=> option.MapFrom(MapMoviesActors));
            CreateMap<MovieCreationDTO, Movie>().ForMember(movie => movie.Poster, options => options.Ignore())
                .ForMember(x=> x.MoviesGenders, option=> option.MapFrom(mappingFunction: MapMoviesGendersCreationDTO))
                .ForMember(x=> x.MoviesActors, option=> option.MapFrom(MapMoviesActorsCreationDTO));
            CreateMap<MoviePatchDTO, Movie>().ReverseMap();

            //RoomMovie
            CreateMap<RoomMovie, RoomMovieDTO>().ForMember(x=> x.Latitude, x=> x.MapFrom(y=> y.Location.Y))
                .ForMember(x => x.Longitude, x => x.MapFrom(y => y.Location.X));
            //CreateMap<RoomMovieDTO, RoomMovie>();
            CreateMap<RoomMovieCreationDTO, RoomMovie>().ForMember(x => x.Location,
                y => y.MapFrom(y=> geometryFactory.CreatePoint(new Coordinate(y.Longitude, y.Latitude))));

            //Reviews
            CreateMap<Review, ReviewDTO >().ForMember(x=> x.UserName, x=> x.MapFrom(y=> y.User.UserName));
            CreateMap<ReviewCreationDTO, Review>();

        }

        private List<MoviesGenders> MapMoviesGendersCreationDTO(MovieCreationDTO movieCreationDTO, Movie movie)
        {
            var listMoviesGenders = new List<MoviesGenders>();

            if (movieCreationDTO.GendersIds == null) return listMoviesGenders;

            foreach(var id in movieCreationDTO.GendersIds)
            {
                listMoviesGenders.Add(new MoviesGenders { GenderId = id });
            }

            return listMoviesGenders;
        }

        private List<MoviesActors> MapMoviesActorsCreationDTO(MovieCreationDTO movieCreationDTO, Movie movie)
        {
            var listMoviesActors = new List<MoviesActors>();
            if (movieCreationDTO.Actors == null) return listMoviesActors;
            foreach(var movieActor in movieCreationDTO.Actors)
            {
                listMoviesActors.Add(new MoviesActors {
                    ActorId = movieActor.ActorId, Character = movieActor.Character});
            }
            return listMoviesActors;
        }

        private List<GenderDTO> MapToMovieWithDetailsDTO(Movie movie, MovieWithDetailsDTO movieWithDetailsDTO)
        {
            List<GenderDTO> listGenders = new List<GenderDTO>();

            if (movie.MoviesGenders == null) return listGenders;

            foreach (var movieGender in movie.MoviesGenders)
            {
                listGenders.Add(new GenderDTO { Id = movieGender.Gender.Id, Name = movieGender.Gender.Name });
            }

            return listGenders;
        }

        private List<MovieActorsDetailsDTO> MapMoviesActors(Movie movie, MovieWithDetailsDTO movieWithDetailsDTO)
        {
            List<MovieActorsDetailsDTO> actorsDetailsDTOs = new List<MovieActorsDetailsDTO>();

            if (movie.MoviesActors == null) return actorsDetailsDTOs;

            foreach(var actor in movie.MoviesActors)
            {
                actorsDetailsDTOs.Add(new MovieActorsDetailsDTO
                {
                    ActorId = actor.ActorId,
                    Character = actor.Character,
                    ActorName = actor.Actor.Name
                });
            }

            return actorsDetailsDTOs;
        }
    }
}
