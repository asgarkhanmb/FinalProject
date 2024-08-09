

using Service.DTOs.Admin.Baskets;

namespace Service.Services.Interfaces
{
    public interface IBasketService
    {
        Task<BasketDto> GetBasketByUserIdAsync(string userId);
        Task AddBasketAsync(BasketDto basketDto);
        Task DeleteBasketAsync(int id);
        Task DeleteProductFromBasket(int productId, int basketId);
    }
}
