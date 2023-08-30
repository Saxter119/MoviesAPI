using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Entities
{
    public class Review
    {
        public int Id { get; set; }
        [StringLength(1500)]
        public string Comment { get; set; }
        [Range(1, 5)]
        [Required]
        public int Punctuation { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public string UserId { get; set; }
        public IdentityUser User { get; set; }


    }
}
