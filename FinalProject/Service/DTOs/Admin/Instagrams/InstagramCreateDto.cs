using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Admin.Instagrams
{
    public class InstagramCreateDto
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string SocialName { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public ICollection<InstagramGallery> InstagramGalleries { get; set; }

        [Required]
        public ICollection<IFormFile> UploadImages { get; set; }
    }
}

