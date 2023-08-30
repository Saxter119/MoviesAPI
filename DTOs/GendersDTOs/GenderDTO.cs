using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs
{
    public class GenderDTO
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(40)]
        public string Name { get; set; }
    }
}
