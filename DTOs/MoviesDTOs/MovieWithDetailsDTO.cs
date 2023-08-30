namespace MoviesAPI.DTOs
{
    public class MovieWithDetailsDTO : MovieDTO
    {
        public List<GenderDTO> GenderDTOs { get; set; }
        public List<MovieActorsDetailsDTO> ActorsDTOs { get; set; }
    }
}
