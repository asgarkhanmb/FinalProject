using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;


namespace Service.DTOs.Admin.Instagrams
{
    public class InstagramEditDto
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string SocialName { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public ICollection<InstagramGallery> InstagramGalleries { get; set; }
        public ICollection<IFormFile> UploadImages { get; set; }
    }
}
