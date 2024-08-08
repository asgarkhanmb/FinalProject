using Repository.Helpers;
using Service.DTOs.Admin.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDto> GetByIdAsync(int? id);
        Task CreateAsync(ProductCreateDto model);
        Task DeleteAsync(int? id);
        Task EditAsync(int? id, ProductEditDto model);
        Task<PaginationResponse<ProductDto>> GetPaginateDataAsync(int page, int take);
        Task<IEnumerable<ProductDto>> Search(string name);
        Task<IEnumerable<ProductDto>> SortBy(string sortKey, bool isDescending);
        Task<IEnumerable<ProductDto>> FilterAsync(string categoryName);
    }
}
