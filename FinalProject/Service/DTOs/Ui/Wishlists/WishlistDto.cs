using System.ComponentModel.DataAnnotations;

namespace Service.DTOs.Ui.Wishlists
{
    public class WishlistDto
    {
        public string AppUserId { get; set; }
        public List<WishlistProductDto> Products { get; set; }
    }
}
