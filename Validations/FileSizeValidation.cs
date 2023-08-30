using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Validations
{
    public class FileSizeValidation : ValidationAttribute
    {
        private readonly int maxImageSizeOnMB;

        public FileSizeValidation(int MaxFileSizeOnMB)
        {
            maxImageSizeOnMB = MaxFileSizeOnMB;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;

            IFormFile formFile = value as IFormFile;

            if (formFile == null) return ValidationResult.Success;

            if (formFile.Length > maxImageSizeOnMB * 1024 * 1024) return new ValidationResult($"The file cann't be bigger than {maxImageSizeOnMB} MegaBytes");

            else return ValidationResult.Success;
        }
    }
}
