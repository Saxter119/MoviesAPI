namespace MoviesAPI.Entities
{
    public class MoviesRoomMovies
    {
        public int MovieId { get; set; }
        public int RoomMovieId { get; set; }
        public Movie Movie { get; set; }
        public RoomMovie RoomMovie { get; set; }
    }
}
