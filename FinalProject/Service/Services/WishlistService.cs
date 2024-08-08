using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Repositories.Interfaces;
using Service.DTOs.Admin.Wishlists;
using Service.Services.Interfaces;


namespace Service.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IProductService _productService;

        public WishlistService(IWishlistRepository wishlistRepository, IProductService productService)
        {
            _wishlistRepository = wishlistRepository;
            _productService = productService;
        }

        public async Task<WishlistDto> GetWishlistByUserIdAsync(string userId)
        {
            var wishlist = await _wishlistRepository.GetByUserIdAsync(userId);
            return ConvertToDto(wishlist);
        }

        public async Task AddWishlistAsync(WishlistDto wishlistDto)
        {
            WishlistDto existWishListDto = await GetWishlistByUserIdAsync(wishlistDto.AppUserId);
            if (existWishListDto == null)
            {
                List<WishlistProduct> wishlistProducts = new List<WishlistProduct>();
                wishlistProducts.Add(new WishlistProduct { ProductId = wishlistDto.ProductId });
                var wishlist = new Wishlist
                {
                    AppUserId = wishlistDto.AppUserId,
                    WishlistProducts = wishlistProducts
                };

                await _wishlistRepository.AddAsync(wishlist);
            }
            else
            {
                Wishlist existWishList = await _wishlistRepository.GetByUserIdAsync(existWishListDto.AppUserId);
                existWishList.WishlistProducts.Add(new WishlistProduct { ProductId = wishlistDto.ProductId });
            }

            await _wishlistRepository.SaveChanges();
        }
        public async Task DeleteProductFromWishList(int productId, int wishListId)
        {
            Wishlist existWishList = await _wishlistRepository.GetByIdAsync(wishListId);
            existWishList.WishlistProducts.RemoveAll(wp => wp.ProductId == productId);

            await _wishlistRepository.SaveChanges();
        }
        public async Task DeleteWishlistAsync(int id)
        {
            await _wishlistRepository.DeleteAsync(id);
            await _wishlistRepository.SaveChanges();
        }
        private WishlistDto ConvertToDto(Wishlist wishlist)
        {
            if (wishlist == null)
            {
                return null;
            }
            return new WishlistDto
            {

                AppUserId = wishlist.AppUserId,
                ProductId =wishlist.WishlistProducts[0].ProductId,

            };
        }
    }
}

