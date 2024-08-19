using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Repositories.Interfaces;
using Service.DTOs.Ui.Baskets;
using Service.Helpers.Exceptions;
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
            if (string.IsNullOrEmpty(basketCreateDto.UserId) || basketCreateDto.ProductId <= 0)
            {
                throw new RequiredException("UserId and ProductId cannot be null or zero.");
            }

            var userExists = await _basketRepository.UserExistsAsync(basketCreateDto.UserId);
            if (!userExists)
            {
                throw new NotFoundException("The provided UserId does not exist.");
            }

            var productExists = await _basketRepository.ProductExistAsync(basketCreateDto.ProductId);
            if (!productExists)
            {
                throw new NotFoundException("The provided ProductId does not exist.");
            }

            var existBasket = await _basketRepository.GetByUserIdAsync(basketCreateDto.UserId);

            if (existBasket == null)
            {
                existBasket = new Basket
                {
                    AppUserId = basketCreateDto.UserId
                };
                existBasket.BasketProducts.Add(new BasketProduct { ProductId = basketCreateDto.ProductId, Quantity = 1 });
                await _basketRepository.AddAsync(existBasket);
            }
            else
            {
                var existingProduct = existBasket.BasketProducts.FirstOrDefault(bp => bp.ProductId == basketCreateDto.ProductId);
                if (existingProduct != null)
                {
                    existingProduct.Quantity++;
                }
                else
                {
                    existBasket.BasketProducts.Add(new BasketProduct { ProductId = basketCreateDto.ProductId, Quantity = 1 });
                }
            }
            await _basketRepository.SaveChangesAsync();
        }


        public async Task IncreaseQuantityAsync(BasketCreateDto basketCreateDto)
        {
            if (string.IsNullOrEmpty(basketCreateDto.UserId) || basketCreateDto.ProductId == 0)
            {
                throw new RequiredException("UserId or ProductId cannot be null or zero.");
            }

            var basket = await _basketRepository.GetByUserIdAsync(basketCreateDto.UserId);

            if (basket == null)
            {
                throw new NotFoundException("Basket not found for the given user.");
            }

            var product = basket.BasketProducts.FirstOrDefault(bp => bp.ProductId == basketCreateDto.ProductId);

            if (product == null)
            {
                throw new NotFoundException("Product not found in the basket.");
            }
            if (product != null)
            {
                product.Quantity++;
                await _basketRepository.SaveChangesAsync();
            }
        }

        public async Task DecreaseQuantityAsync(BasketCreateDto basketCreateDto)
        {

            if (string.IsNullOrEmpty(basketCreateDto.UserId) || basketCreateDto.ProductId == 0)
            {
                throw new RequiredException("UserId or ProductId cannot be null or zero.");
            }

            var basket = await _basketRepository.GetByUserIdAsync(basketCreateDto.UserId);

            if (basket == null)
            {
                throw new NotFoundException("Basket not found for the given user.");
            }

            var product = basket.BasketProducts.FirstOrDefault(bp => bp.ProductId == basketCreateDto.ProductId);

            if (product == null)
            {
                throw new NotFoundException("Product not found in the basket.");
            }

            product.Quantity--;

            if (product.Quantity == 0)
            {
                basket.BasketProducts.Remove(product);
            }

            await _basketRepository.SaveChangesAsync();
        }

        private BasketDto ConvertToDto(Basket basket)
        {
            if (basket == null) return null;
            return new BasketDto
            {
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
            if (string.IsNullOrEmpty(userId) || productId == 0)
            {
                throw new RequiredException("UserId or ProductId cannot be null or zero.");
            }
            var basket = await _basketRepository.GetByUserIdAsync(userId);
            if (basket == null)
            {
                throw new NotFoundException("User not found");
            }
            if (basket != null)
            {
                var product = basket.BasketProducts.FirstOrDefault(bp => bp.ProductId == productId);
                if (product == null)
                {
                    throw new NotFoundException("Product not found");
                }
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
