using System.ComponentModel.DataAnnotations;

namespace Service.DTOs.Admin.Wishlists
{
    public class WishlistDto
    { 

        [Required]
        public string AppUserId { get; set; }
        [Required]
        public int ProductId { get; set; }
    }
}
