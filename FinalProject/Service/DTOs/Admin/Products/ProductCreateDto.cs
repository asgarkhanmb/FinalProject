using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;


namespace Service.DTOs.Admin.Products
{
    public class ProductCreateDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [SwaggerSchema(ReadOnly = true)]
        public ICollection<ProductImage> ProductImages { get; set; }

        [Required]
        public ICollection<IFormFile> UploadImages { get; set; }
    }
}
