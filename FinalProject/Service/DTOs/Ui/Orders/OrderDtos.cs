using System.ComponentModel.DataAnnotations;

namespace Service.DTOs.Ui.Orders
{
    public class CheckoutDto
    {
        [Required, MaxLength(100)]
        public string FullName { get; set; }

        [Required, MaxLength(30)]
        public string Phone { get; set; }

        [Required, MaxLength(300)]
        public string Address { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }
    }

    public class CheckoutResponseDto
    {
        public int OrderId { get; set; }
        // Stripe Checkout səhifəsinin ünvanı — frontend istifadəçini bura yönləndirir
        public string Url { get; set; }
    }

    public class OrderItemDto
    {
        public int? ProductId { get; set; }
        public string ProductName { get; set; }
        public string? Image { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string? Note { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? PaidDate { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderStatsDto
    {
        public int TotalOrders { get; set; }
        public int PaidOrders { get; set; }
        public int PendingOrders { get; set; }
        public decimal Revenue { get; set; }
        public int Products { get; set; }
        public int Categories { get; set; }
        public int Users { get; set; }
        public int Messages { get; set; }
        public int Subscribers { get; set; }
        public List<OrderDto> RecentOrders { get; set; } = new();
    }
}
