using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Service.DTOs.Admin.Teams
{
    public class TeamEditDto
    {
        [Required]
        public string Title { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public string? Image { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Position { get; set; }
        public IFormFile UploadImage { get; set; }
    }
}
