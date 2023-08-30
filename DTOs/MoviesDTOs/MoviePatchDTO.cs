using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs
{
    public class MoviePatchDTO
    {
        [Required]
        [StringLength(300, ErrorMessage = "Name must be between 1 and 300 characters", MinimumLength = 1)]
        public string Name { get; set; }
        public bool OnCinemas { get; set; }
        public DateTime DateRelease { get; set; }
    }
}
