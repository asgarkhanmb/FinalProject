using Service.DTOs.Ui.Baskets;

namespace Service.Services.Interfaces
{
    public interface IBasketService
    {
        Task<BasketDto> GetBasketByUserIdAsync(string userId);
        Task AddBasketAsync(BasketCreateDto basketCreateDto);
        Task IncreaseQuantityAsync(int productId, string userId);
        Task DecreaseQuantityAsync(int productId, string userId);
        Task DeleteProductFromBasketAsync(int productId, string userId);
        Task<List<BasketDto>> GetAllBasketsAsync();
    }
}
