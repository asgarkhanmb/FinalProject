using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Ui.Baskets
{
    public class BasketCreateDto
    {
        public string UserId { get; set; }
        public int ProductId { get; set; }
    }
}
