using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Repository.Repositories.Interfaces;
using Service.DTOs.Ui.Orders;
using Service.Helpers.Account;
using Service.Helpers.Exceptions;
using Service.Services.Interfaces;
using Stripe.Checkout;

namespace Service.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IBasketRepository _basketRepo;
        private readonly IProductRepository _productRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IContactRepository _contactRepo;
        private readonly ISubscribeRepository _subscribeRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly StripeSettings _stripe;
        private readonly string _clientUrl;

        public OrderService(IOrderRepository orderRepo,
                            IBasketRepository basketRepo,
                            IProductRepository productRepo,
                            ICategoryRepository categoryRepo,
                            IContactRepository contactRepo,
                            ISubscribeRepository subscribeRepo,
                            UserManager<AppUser> userManager,
                            IOptions<StripeSettings> stripe,
                            IConfiguration configuration)
        {
            _orderRepo = orderRepo;
            _basketRepo = basketRepo;
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
            _contactRepo = contactRepo;
            _subscribeRepo = subscribeRepo;
            _userManager = userManager;
            _stripe = stripe.Value;
            _clientUrl = (configuration["ClientUrl"] ?? "http://127.0.0.1:5500").TrimEnd('/');
        }

        private Stripe.RequestOptions StripeRequest()
        {
            if (string.IsNullOrWhiteSpace(_stripe.SecretKey))
                throw new ApplicationException("Ödəniş sistemi qurulmayıb. appsettings faylına Stripe test açarını (Stripe:SecretKey) əlavə edin.");
            return new Stripe.RequestOptions { ApiKey = _stripe.SecretKey };
        }

        public async Task<CheckoutResponseDto> CheckoutAsync(string userId, CheckoutDto model)
        {
            if (string.IsNullOrWhiteSpace(model.FullName) ||
                string.IsNullOrWhiteSpace(model.Phone) ||
                string.IsNullOrWhiteSpace(model.Address))
                throw new ApplicationException("Ad soyad, telefon və ünvan mütləqdir.");

            var user = await _userManager.FindByIdAsync(userId) ?? throw new UnauthorizeException("User not found");

            var basket = await _basketRepo.GetByUserIdAsync(userId);
            if (basket == null || basket.BasketProducts.Count == 0)
                throw new ApplicationException("Səbətiniz boşdur.");

            var ids = basket.BasketProducts.Select(b => b.ProductId).ToList();
            var products = await _productRepo.FindAllWithIncludes()
                .Include(p => p.ProductImages)
                .Where(p => ids.Contains(p.Id))
                .AsNoTracking()
                .ToListAsync();

            var order = new Order
            {
                AppUserId = userId,
                FullName = model.FullName.Trim(),
                Phone = model.Phone.Trim(),
                Address = model.Address.Trim(),
                Note = string.IsNullOrWhiteSpace(model.Note) ? null : model.Note.Trim(),
                Status = OrderStatuses.Pending
            };

            foreach (var item in basket.BasketProducts)
            {
                var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product == null) continue;
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = item.Quantity,
                    Image = product.ProductImages?.FirstOrDefault(i => i.IsMain)?.Image
                            ?? product.ProductImages?.FirstOrDefault()?.Image
                });
            }

            if (order.OrderItems.Count == 0)
                throw new ApplicationException("Səbətiniz boşdur.");

            order.TotalPrice = order.OrderItems.Sum(i => i.Price * i.Quantity);

            // Stripe Checkout sessiyası (test rejimində kart: 4242 4242 4242 4242)
            var options = new SessionCreateOptions
            {
                Mode = "payment",
                CustomerEmail = user.Email,
                SuccessUrl = $"{_clientUrl}/order-success.html?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{_clientUrl}/checkout.html?canceled=1",
                LineItems = order.OrderItems.Select(i => new SessionLineItemOptions
                {
                    Quantity = i.Quantity,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = _stripe.Currency,
                        UnitAmount = (long)Math.Round(i.Price * 100),
                        ProductData = new SessionLineItemPriceDataProductDataOptions { Name = i.ProductName }
                    }
                }).ToList(),
                Metadata = new Dictionary<string, string> { { "userId", userId } }
            };

            Session session;
            try
            {
                session = await new SessionService().CreateAsync(options, StripeRequest());
            }
            catch (Stripe.StripeException ex)
            {
                throw new ApplicationException($"Ödəniş başladıla bilmədi: {ex.StripeError?.Message ?? ex.Message}");
            }

            order.StripeSessionId = session.Id;
            await _orderRepo.AddAsync(order);
            await _orderRepo.SaveChangesAsync();

            return new CheckoutResponseDto { OrderId = order.Id, Url = session.Url };
        }

        public async Task<OrderDto> ConfirmPaymentAsync(string userId, string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId)) throw new ApplicationException("Ödəniş sessiyası tapılmadı.");

            var order = await _orderRepo.GetBySessionIdAsync(sessionId);
            if (order == null || order.AppUserId != userId) throw new NotFoundException("Sifariş tapılmadı");

            if (order.Status == OrderStatuses.Pending)
            {
                Session session;
                try
                {
                    session = await new SessionService().GetAsync(sessionId, null, StripeRequest());
                }
                catch (Stripe.StripeException ex)
                {
                    throw new ApplicationException($"Ödəniş yoxlanıla bilmədi: {ex.StripeError?.Message ?? ex.Message}");
                }

                if (session.PaymentStatus == "paid")
                {
                    order.Status = OrderStatuses.Paid;
                    order.PaidDate = DateTime.Now;

                    // Ödəniş uğurludursa səbəti boşaldırıq
                    var basket = await _basketRepo.GetByUserIdAsync(userId);
                    basket?.BasketProducts.Clear();

                    await _orderRepo.SaveChangesAsync();
                }
            }

            return ToDto(order);
        }

        public async Task<List<OrderDto>> GetMyOrdersAsync(string userId)
        {
            return (await _orderRepo.GetByUserIdAsync(userId)).Select(ToDto).ToList();
        }

        public async Task<List<OrderDto>> GetAllAsync()
        {
            return (await _orderRepo.GetAllWithItemsAsync()).Select(ToDto).ToList();
        }

        public async Task<OrderDto> GetByIdAsync(int id)
        {
            var order = await _orderRepo.GetWithItemsAsync(id) ?? throw new NotFoundException("Sifariş tapılmadı");
            return ToDto(order);
        }

        public async Task ChangeStatusAsync(int id, string status)
        {
            var normalized = OrderStatuses.All.FirstOrDefault(s => s.Equals(status, StringComparison.OrdinalIgnoreCase))
                ?? throw new ApplicationException($"Yanlış status. Mümkün dəyərlər: {string.Join(", ", OrderStatuses.All)}");

            var order = await _orderRepo.GetWithItemsAsync(id) ?? throw new NotFoundException("Sifariş tapılmadı");
            order.Status = normalized;
            if (normalized == OrderStatuses.Paid && order.PaidDate == null) order.PaidDate = DateTime.Now;
            await _orderRepo.SaveChangesAsync();
        }

        public async Task<OrderStatsDto> GetStatsAsync()
        {
            var orders = await _orderRepo.GetAllWithItemsAsync();
            var paidStatuses = new[] { OrderStatuses.Paid, OrderStatuses.Preparing, OrderStatuses.Delivered };

            return new OrderStatsDto
            {
                TotalOrders = orders.Count,
                PaidOrders = orders.Count(o => paidStatuses.Contains(o.Status)),
                PendingOrders = orders.Count(o => o.Status == OrderStatuses.Pending),
                Revenue = orders.Where(o => paidStatuses.Contains(o.Status)).Sum(o => o.TotalPrice),
                Products = await _productRepo.FindAllWithIncludes().CountAsync(),
                Categories = await _categoryRepo.FindAllWithIncludes().CountAsync(),
                Users = await _userManager.Users.CountAsync(),
                Messages = await _contactRepo.FindAllWithIncludes().CountAsync(),
                Subscribers = await _subscribeRepo.FindAllWithIncludes().CountAsync(),
                RecentOrders = orders.Take(5).Select(ToDto).ToList()
            };
        }

        private static OrderDto ToDto(Order o) => new()
        {
            Id = o.Id,
            FullName = o.FullName,
            Phone = o.Phone,
            Address = o.Address,
            Note = o.Note,
            TotalPrice = o.TotalPrice,
            Status = o.Status,
            CreatedDate = o.CreatedDate,
            PaidDate = o.PaidDate,
            UserName = o.AppUser?.UserName,
            Email = o.AppUser?.Email,
            Items = o.OrderItems.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Image = i.Image,
                Price = i.Price,
                Quantity = i.Quantity
            }).ToList()
        };
    }
}
