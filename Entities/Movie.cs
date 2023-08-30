using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Entities
{
    public class Movie
    {
        public int Id{ get; set; }
        [Required]
        [StringLength(300, ErrorMessage = "Name must be between 1 and 300 characters",  MinimumLength = 1)]
        public string Name{ get; set; }
        public bool OnCinemas { get; set; }
        public DateTime DateRelease { get; set; }
        public string Poster { get; set; }
        public List<Review> Reviews { get; set; }
        public List<MoviesGenders> MoviesGenders { get; set; }
        public List<MoviesActors> MoviesActors { get; set; }
        public List<MoviesRoomMovies> MoviesRoomMovies { get; set; }

    }
}
