using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Repositories.Interfaces;
using Service.DTOs.Ui.Baskets;
using Service.Services.Interfaces;

namespace Service.Services
{
    public class BasketService : IBasketService
    {
        private readonly IBasketRepository _basketRepository;

        public BasketService(IBasketRepository basketRepository)
        {
            _basketRepository = basketRepository;
        }

        public async Task<BasketDto> GetBasketByUserIdAsync(string userId)
        {
            var basket = await _basketRepository.GetByUserIdAsync(userId);
            return ConvertToDto(basket);
        }

        public async Task AddBasketAsync(BasketCreateDto basketCreateDto)
        {
            var basket = await _basketRepository.GetByUserIdAsync(basketCreateDto.UserId);

            if (basket == null)
            {
                basket = new Basket
                {
                    AppUserId = basketCreateDto.UserId
                };
                basket.BasketProducts.Add(new BasketProduct { ProductId = basketCreateDto.ProductId, Quantity = 1 });
                await _basketRepository.AddAsync(basket);
            }
            else
            {
                var existingProduct = basket.BasketProducts.FirstOrDefault(bp => bp.ProductId == basketCreateDto.ProductId);
                if (existingProduct != null)
                {
                    existingProduct.Quantity++;
                }
                else
                {
                    basket.BasketProducts.Add(new BasketProduct { ProductId = basketCreateDto.ProductId, Quantity = 1 });
                }
            }
            await _basketRepository.SaveChangesAsync();
        }


        public async Task IncreaseQuantityAsync(int productId, string userId)
        {
            var basket = await _basketRepository.GetByUserIdAsync(userId);
            var product = basket.BasketProducts.FirstOrDefault(bp => bp.ProductId == productId);
            if (product != null)
            {
                product.Quantity++;
                await _basketRepository.SaveChangesAsync();
            }
        }

        public async Task DecreaseQuantityAsync(int productId, string userId)
        {
            var basket = await _basketRepository.GetByUserIdAsync(userId);
            var product = basket.BasketProducts.FirstOrDefault(bp => bp.ProductId == productId);
            if (product != null)
            {
                product.Quantity--;
                if (product.Quantity == 0)
                {
                    basket.BasketProducts.Remove(product);
                }
                await _basketRepository.SaveChangesAsync();
            }
        }

        private BasketDto ConvertToDto(Basket basket)
        {
            if (basket == null) return null;
            return new BasketDto
            {
                Id = basket.Id,
                AppUserId = basket.AppUserId,
                BasketProducts = basket.BasketProducts.Select(bp => new BasketProductDto
                {
                    ProductId = bp.ProductId,
                    Quantity = bp.Quantity
                }).ToList(),
                TotalProductCount = basket.BasketProducts.Sum(bp => bp.Quantity)
            };
        }

        public async Task DeleteProductFromBasketAsync(int productId, string userId)
        {
            var basket = await _basketRepository.GetByUserIdAsync(userId);
            if (basket != null)
            {
                var product = basket.BasketProducts.FirstOrDefault(bp => bp.ProductId == productId);
                if (product != null)
                {
                    basket.BasketProducts.Remove(product);
                    await _basketRepository.SaveChangesAsync();
                }
            }
        }

        public async Task<List<BasketDto>> GetAllBasketsAsync()
        {
            var baskets = await _basketRepository.FindAllWithIncludes().Include(m => m.BasketProducts).ToListAsync();
            return baskets.Select(b => ConvertToDto(b)).ToList();
        }
    }
}
