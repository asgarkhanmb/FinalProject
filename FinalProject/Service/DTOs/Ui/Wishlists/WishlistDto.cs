using System.ComponentModel.DataAnnotations;

namespace Service.DTOs.Ui.Wishlists
{
    public class WishlistDto
    {


        public string AppUserId { get; set; }

        public int ProductId { get; set; }
    }
}
