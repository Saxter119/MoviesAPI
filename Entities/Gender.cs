using MoviesAPI.Controllers.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Entities
{
    public class Gender : IId
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(40)]
        public string Name{ get; set; }
        public List<MoviesGenders> MoviesGenders { get; set; }
    }
}
