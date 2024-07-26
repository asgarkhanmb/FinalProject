

namespace Domain.Entities
{
    public class ProductImage
    {
        public int Id { get; set; }
        public bool IsMain { get; set; }=false;
        public string Image { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }

    }
}
