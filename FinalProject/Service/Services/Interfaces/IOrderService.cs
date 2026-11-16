using Service.DTOs.Ui.Orders;

namespace Service.Services.Interfaces
{
    public interface IOrderService
    {
        Task<CheckoutResponseDto> CheckoutAsync(string userId, CheckoutDto model);
        Task<OrderDto> ConfirmPaymentAsync(string userId, string sessionId);
        Task<List<OrderDto>> GetMyOrdersAsync(string userId);

        // Admin
        Task<List<OrderDto>> GetAllAsync();
        Task<OrderDto> GetByIdAsync(int id);
        Task ChangeStatusAsync(int id, string status);
        Task<OrderStatsDto> GetStatsAsync();
    }
}
