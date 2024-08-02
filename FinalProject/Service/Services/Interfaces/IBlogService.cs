using Repository.Helpers;
using Service.DTOs.Admin.Blogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Interfaces
{
    public interface IBlogService
    {
        Task<IEnumerable<BlogDto>> GetAllAsync();
        Task<BlogDto> GetByIdAsync(int? id);
        Task CreateAsync(BlogCreateDto model);
        Task DeleteAsync(int? id);
        Task EditAsync(int? id, BlogEditDto model);
        Task<PaginationResponse<BlogDto>> GetPaginateDataAsync(int page, int take);
    }
}
