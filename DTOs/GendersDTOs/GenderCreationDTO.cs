using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs
{
    public class GenderCreationDTO
    {
        [Required]
        [MaxLength(40)]
        public string Name{ get; set; }
    }
}
