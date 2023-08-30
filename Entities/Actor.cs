using MoviesAPI.Controllers.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Entities
{
    public class Actor : IId
    {
        public int Id{ get; set; }
        [Required]
        [MaxLength(120)]
        public string Name{ get; set; }
        public DateTime BirthDate { get; set; }
        public string Photo { get; set; }
        public List<MoviesActors> MoviesActors { get; set; }
    }
}
