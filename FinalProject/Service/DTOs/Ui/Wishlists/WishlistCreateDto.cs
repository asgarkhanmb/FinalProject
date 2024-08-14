using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Ui.Wishlists
{
    public class WishlistCreateDto
    {
        public string AppUserId { get; set; }
        public int ProductId { get; set; }
    }
}
