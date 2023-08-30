using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs.ReviewsDTOs
{
    public class ReviewCreationDTO
    {
        [StringLength(1500)]
        public string Comment { get; set; }
        [Required]
        public int Punctuation { get; set; }
    }
}
