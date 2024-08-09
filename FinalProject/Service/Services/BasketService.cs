using Domain.Entities;
using Repository.Repositories.Interfaces;
using Service.DTOs.Admin.Baskets;
using Service.Services.Interfaces;


namespace Service.Services
{
    public class BasketService : IBasketService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IProductService _productService;


        public BasketService(IBasketRepository basketRepository,
                             IProductService productService)
        {
            _basketRepository = basketRepository;
            _productService = productService;
        }

        public async Task AddBasketAsync(BasketDto basketDto)
        {
            BasketDto existBasketDto = await GetBasketByUserIdAsync(basketDto.AppUserId);
            if (existBasketDto == null)
            {
                List<BasketProduct>basketProducts = new List<BasketProduct>();
                basketProducts.Add(new BasketProduct { ProductId = basketDto.ProductId });
                var basket = new Basket
                {
                    AppUserId = basketDto.AppUserId,
                    BasketProducts = basketProducts
                };

                await _basketRepository.AddAsync(basket);
            }
            else
            {
                Basket existBsket = await _basketRepository.GetByUserIdAsync(existBasketDto.AppUserId);
                existBsket.BasketProducts.Add(new BasketProduct { ProductId = basketDto.ProductId });
            }
            await _basketRepository.SaveChanges();

        }

        public async Task DeleteBasketAsync(int id)
        {
            await _basketRepository.DeleteAsync(id);
            await _basketRepository.SaveChanges();
        }

        public async Task DeleteProductFromBasket(int productId, int basketId)
        {
            Basket existBasket = await _basketRepository.GetByIdAsync(basketId);
            existBasket.BasketProducts.RemoveAll(p => p.ProductId == productId);

            await _basketRepository.SaveChanges();
        }

        public async Task<BasketDto> GetBasketByUserIdAsync(string userId)
        {
           var basket = await _basketRepository.GetByUserIdAsync(userId);
            return ConvertToDto(basket);
        }

        private BasketDto ConvertToDto(Basket basket)
        {
            if (basket == null)
            {
                return null;
            }
            return new BasketDto
            {

                AppUserId = basket.AppUserId,
                ProductId = basket.BasketProducts[0].ProductId,

            };
        }
    }
}
