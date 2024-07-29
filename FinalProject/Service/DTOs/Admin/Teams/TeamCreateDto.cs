using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Admin.Teams
{
    public class TeamCreateDto
    {
        [Required]
        public string Title { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public string? Image { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Position { get; set; }
        [Required]
        public IFormFile UploadImage { get; set; }
        [Required]
        public List<int>?SocialId { get; set; }
    }
}
