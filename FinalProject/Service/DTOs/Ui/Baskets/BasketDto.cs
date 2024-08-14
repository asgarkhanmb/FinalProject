using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Ui.Baskets
{
    public class BasketDto
    {
        public string AppUserId { get; set; }
        public List<BasketProductDto> BasketProducts { get; set; }
        public int TotalProductCount { get; set; }


    }
}
