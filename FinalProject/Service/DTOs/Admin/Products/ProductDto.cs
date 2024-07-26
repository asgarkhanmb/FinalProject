using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Admin.Products
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> Images { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; }
        
    }
}
