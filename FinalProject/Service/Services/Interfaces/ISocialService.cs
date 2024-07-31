using Repository.Helpers;
using Service.DTOs.Admin.Socials;

namespace Service.Services.Interfaces
{
    public interface ISocialService
    {
        Task<IEnumerable<SocialDto>> GetAllAsync();
        Task<SocialDto> GetByIdAsync(int? id);
        Task CreateAsync(SocialCreateDto model);
        Task DeleteAsync(int? id);
        Task EditAsync(int? id, SocialEditDto model);
        Task<PaginationResponse<SocialDto>> GetPaginateDataAsync(int page, int take);
    }
}
