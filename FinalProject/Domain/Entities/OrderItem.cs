namespace Domain.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        // Məhsul sonradan silinsə də sifariş tarixçəsi qalsın deyə ad/qiymət/şəkil burada saxlanılır
        public int? ProductId { get; set; }
        public Product? Product { get; set; }

        public string ProductName { get; set; }
        public string? Image { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
