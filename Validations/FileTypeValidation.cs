using MoviesAPI.Validations.ValidationHelpers;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Validations
{
    public class FileTypeValidation : ValidationAttribute
    {
        private readonly string[] validTypes;

        public FileTypeValidation(string[] validTypes)
        {
            this.validTypes = validTypes;
        }

        public FileTypeValidation(FileGroupType fileGroupType)
        {
            if(fileGroupType == FileGroupType.Image)
            {
                validTypes = new string []{ "image/jpeg", "image/png", "image/gif" };
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;

            IFormFile formFile = value as IFormFile;

            if (formFile == null) return ValidationResult.Success;

            if (validTypes.Any(item => item == formFile.ContentType)) //Another way for this is: if(!validTypes.Contains(formFile.ContentType))
            {
                return ValidationResult.Success;
            }
            else return new ValidationResult($"This kind of file is not accepted. Must be one of these:{string.Join(", ", validTypes)}");
        }
    }
}
