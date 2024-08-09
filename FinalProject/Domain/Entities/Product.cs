using Domain.Common;


namespace Domain.Entities
{
    public class Product :BaseEntity
    {
        public string Name { get; set; }
        public decimal Price { get; set; }  
        public Category Category { get; set; }
        public int CategoryId { get; set; }
        public List<ProductImage> ProductImages { get; set; }
        public List<WishlistProduct> WishlistProducts { get; set; }
        public List<BasketProduct> BasketProducts { get; set; }

    }
}
