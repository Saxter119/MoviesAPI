using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs.RoomMovies
{
    public class RoomMovieNearFilterDTO
    {
        [Range(-90, 90)]
        public double Latitude { get; set; }
        [Range(-180, 180)]
        public double Longitude { get; set; }
        
        private int KmDistance = 10;
        
        private int MaxDistanceOnKm = 100;
        public int DistanceOnKms { get => KmDistance; set { KmDistance = value > MaxDistanceOnKm ? MaxDistanceOnKm : value; } }
    }
}
