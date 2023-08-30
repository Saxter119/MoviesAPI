using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs
{
    public class ActorPatchDTO
    {
        [Required]
        [MaxLength(120)]
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
