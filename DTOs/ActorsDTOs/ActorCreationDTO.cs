using MoviesAPI.Validations;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs
{
    public class ActorCreationDTO : ActorPatchDTO
    {
        [FileSizeValidation(MaxFileSizeOnMB:4)]
        [FileTypeValidation(Validations.ValidationHelpers.FileGroupType.Image)]
        public IFormFile Photo { get; set; }
    }
}
