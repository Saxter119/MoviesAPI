using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Services;
using MoviesAPI.Validations;

namespace MoviesAPI.DTOs
{
    public class MovieCreationDTO : MoviePatchDTO
    {
        [FileTypeValidation(Validations.ValidationHelpers.FileGroupType.Image)]
        [FileSizeValidation(5)]
        public IFormFile Poster { get; set; }
        [ModelBinder(binderType:typeof(TypeBinder<List<int>>))]
        public List<int> GendersIds { get; set; }
        [ModelBinder(typeof(TypeBinder<List<MovieActorsCreationDTO>>))]
        public List<MovieActorsCreationDTO> Actors { get; set; }
    }
}
