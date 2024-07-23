using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;


namespace Service.DTOs.Admin.Categories
{
    public class CategoryCreateDto
    {
        [Required]
        public string Name { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public string? Icon { get; set; }
        [Required]
        public IFormFile UploadImage { get; set; }
    }
}
