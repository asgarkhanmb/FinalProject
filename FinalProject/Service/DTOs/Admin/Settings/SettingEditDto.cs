using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;


namespace Service.DTOs.Admin.Settings
{
    public class SettingEditDto
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Phone { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public string? Logo { get; set; }
        public IFormFile UploadImage { get; set; }
    }
}
