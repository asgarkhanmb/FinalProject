using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;


namespace Service.DTOs.Admin.Products
{
    public class ProductEditDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int CategoryId { get; set; }

        public ICollection<IFormFile> UploadImages { get; set; }
    }
}
