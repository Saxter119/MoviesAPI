using MoviesAPI.Entities;

namespace MoviesAPI.DTOs
{
    public class MovieDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool OnCinemas { get; set; }
        public DateTime DateRelease { get; set; }
        public string Poster { get; set; }
    }
}
