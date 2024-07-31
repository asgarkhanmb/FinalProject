using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;


namespace Service.DTOs.Admin.Testimonials
{
    public class TestimonialEditDto
    {

        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public string? Image { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string City { get; set; }
        public IFormFile UploadImage { get; set; }
    }
}
