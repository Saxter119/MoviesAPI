using MoviesAPI.Controllers.Interfaces;
using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;

namespace MoviesAPI.Entities
{
    public class RoomMovie : IId
    {
        public int Id { get; set; }
        [Required]
        [StringLength(120)]
        public string Name { get; set; }
        public Point Location{ get; set; }
        public List<MoviesRoomMovies> MoviesRoomMovies { get; set; } //falta asignarle que introduzca los valores en la tabla
    }
}
