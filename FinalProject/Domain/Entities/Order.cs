using Domain.Common;

namespace Domain.Entities
{
    public class Order : BaseEntity
    {
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string? Note { get; set; }

        public decimal TotalPrice { get; set; }

        // Pending, Paid, Preparing, Delivered, Cancelled
        public string Status { get; set; } = OrderStatuses.Pending;

        public string? StripeSessionId { get; set; }
        public DateTime? PaidDate { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new();
    }

    public static class OrderStatuses
    {
        public const string Pending = "Pending";
        public const string Paid = "Paid";
        public const string Preparing = "Preparing";
        public const string Delivered = "Delivered";
        public const string Cancelled = "Cancelled";

        public static readonly string[] All = { Pending, Paid, Preparing, Delivered, Cancelled };
    }
}
