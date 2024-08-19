using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Repositories;
using Repository.Repositories.Interfaces;
using Service.DTOs.Ui.Baskets;
using Service.DTOs.Ui.Wishlists;
using Service.Helpers.Exceptions;
using Service.Services.Interfaces;


namespace Service.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IWishlistRepository _wishlistRepository;

        public WishlistService(IWishlistRepository wishlistRepository)
        {
            _wishlistRepository = wishlistRepository;
        }

        public async Task<WishlistDto> GetWishlistByUserIdAsync(string userId)
        {

            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
            }

            var wishlist = await _wishlistRepository.GetByUserIdAsync(userId);

            if (wishlist == null)
            {
                throw new NotFoundException("User Not Found");
            }

            return ConvertToDto(wishlist);

        }

        public async Task AddWishlistAsync(WishlistCreateDto wishlistDto)
        {
            if (string.IsNullOrEmpty(wishlistDto.AppUserId) || wishlistDto.ProductId <= 0)
            {
                throw new RequiredException("UserId and ProductId cannot be null or zero.");
            }

            var userExists = await _wishlistRepository.UserExistsAsync(wishlistDto.AppUserId);
            if (!userExists)
            {
                throw new NotFoundException("The UserId does not exist.");
            }

            var productExists = await _wishlistRepository.ProductExistAsync(wishlistDto.ProductId);
            if (!productExists)
            {
                throw new NotFoundException("The ProductId does not exist.");
            }

            var existingWishlist = await _wishlistRepository.GetByUserIdAsync(wishlistDto.AppUserId);

            if (existingWishlist == null)
            {
     
                var newWishlist = new Wishlist
                {
                    AppUserId = wishlistDto.AppUserId,
                    WishlistProducts = new List<WishlistProduct>
            {
                new WishlistProduct
                {
                    ProductId = wishlistDto.ProductId
                }
            }
                };

                await _wishlistRepository.AddAsync(newWishlist);
            }
            else
            {

                if (existingWishlist.WishlistProducts.Any(wp => wp.ProductId == wishlistDto.ProductId))
                {
                    throw new RequiredException("This product is already in your wishlist.");
                }

                existingWishlist.WishlistProducts.Add(new WishlistProduct
                {
                    ProductId = wishlistDto.ProductId
                });

                await _wishlistRepository.UpdateAsync(existingWishlist);
            }

            await _wishlistRepository.SaveChanges();
        }

        public async Task DeleteProductFromWishlistAsync(string userId, int productId)
        {
            if (string.IsNullOrEmpty(userId) || productId == 0)
            {
                throw new RequiredException("UserId or ProductId cannot be null or zero.");
            }

            var wishlist = await _wishlistRepository.GetByUserIdAsync(userId);
            if (wishlist == null)
            {
                throw new NotFoundException("User not found");
            }

            var productToRemove = wishlist.WishlistProducts.FirstOrDefault(bp => bp.ProductId == productId);
            if (productToRemove == null)
            {
                throw new NotFoundException("Product not found");
            }

            wishlist.WishlistProducts.Remove(productToRemove);
            await _wishlistRepository.UpdateAsync(wishlist);
        }

        public async Task<List<WishlistCreateDto>> GetAllWishlistsAsync()
        {
            var wishlists = await _wishlistRepository.FindAllWithIncludes()
                .Include(w => w.WishlistProducts)
                .ThenInclude(wp => wp.Product)
                .ToListAsync();
            var allWishlists = new List<WishlistCreateDto>();

            foreach (var wishlist in wishlists)
            {
                foreach (var wishlistProduct in wishlist.WishlistProducts)
                {
                    if (wishlistProduct.Product != null)
                    {
                        var wishlistCreateDto = new WishlistCreateDto
                        {
                            AppUserId = wishlist.AppUserId,
                            ProductId = wishlistProduct.ProductId,
                        };

                        allWishlists.Add(wishlistCreateDto);
                    }
                }
            }

            return allWishlists;
        }


        private WishlistDto ConvertToDto(Wishlist wishlist)
        {
            if (wishlist == null || wishlist.WishlistProducts == null)
            {
                return new WishlistDto
                {
                    AppUserId = wishlist?.AppUserId,
                    Products = new List<WishlistProductDto>()
                };
            }

            return new WishlistDto
            {
                AppUserId = wishlist.AppUserId,
                Products = wishlist.WishlistProducts.Select(product => new WishlistProductDto
                {
                    ProductId = product.ProductId,
                    ProductName = product.Product != null ? product.Product.Name : "Unknown",
                    ProductPrice = product.Product != null ? product.Product.Price : 0.0m,
                    ProductImage = product.Product != null ? product.Product.ProductImages.FirstOrDefault()?.Image : "NoImage"
                }).ToList()
            };
        }
    }
}

