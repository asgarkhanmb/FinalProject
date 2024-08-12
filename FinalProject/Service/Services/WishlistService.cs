using Domain.Entities;
using Repository.Repositories.Interfaces;
using Service.DTOs.Ui.Wishlists;
using Service.Helpers.Exceptions;
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
            Wishlist wishList = await _wishlistRepository.GetByUserIdAsync(wishlistDto.AppUserId);
            if (wishList != null)
            {
                bool productExists = wishList.WishlistProducts.Any(p => p.ProductId == wishlistDto.ProductId);
                if (productExists)
                {
                    throw new RequiredException("This product has already been added to your wishlist.");
                }
               
            }
            else if (wishList == null)
            {
                throw new NotFoundException("Not found");
            }
            else
            {
                wishList = new Wishlist
                {
                    AppUserId = wishlistDto.AppUserId,
                    WishlistProducts = new List<WishlistProduct>
                {
                new WishlistProduct { ProductId = wishlistDto.ProductId }
                }
                };

                await _wishlistRepository.AddAsync(wishList);
            }

            if (wishList.WishlistProducts.All(p => p.ProductId != wishlistDto.ProductId))
            {
                wishList.WishlistProducts.Add(new WishlistProduct { ProductId = wishlistDto.ProductId });
            }

            await _wishlistRepository.SaveChanges();
        }
        public async Task DeleteProductFromWishList(int productId, int wishListId)
        {
            ArgumentNullException.ThrowIfNull(nameof(wishListId));

            var wishList = await _wishlistRepository.GetById((int)wishListId) ?? throw new NotFoundException("Data not found");
            Wishlist existWishList = await _wishlistRepository.GetByIdAsync(wishListId);
            existWishList.WishlistProducts.RemoveAll(wp => wp.ProductId == productId);

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
                ProductId = wishlist.WishlistProducts[0].ProductId,

            };
        }
    }
}

