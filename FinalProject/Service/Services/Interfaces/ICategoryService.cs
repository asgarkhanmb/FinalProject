using Repository.Helpers;
using Service.DTOs.Admin.Categories;
using Service.DTOs.Admin.Products;


namespace Service.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllAsync();
        Task<CategoryDto> GetByIdAsync(int? id);
        Task CreateAsync(CategoryCreateDto model);
        Task DeleteAsync(int? id);
        Task EditAsync(int? id, CategoryEditDto model);
        Task<IEnumerable<CategoryDto>> Search(string name);
        Task<PaginationResponse<CategoryDto>> GetPaginateDataAsync(int page, int take);
    }
}
