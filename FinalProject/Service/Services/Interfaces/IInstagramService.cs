using Service.DTOs.Admin.Instagrams;

namespace Service.Services.Interfaces
{
    public interface IInstagramService
    {
        Task<IEnumerable<InstagramDto>> GetAllAsync();
        Task<InstagramDto> GetByIdAsync(int? id);
        Task CreateAsync(InstagramCreateDto model);
        Task DeleteAsync(int? id);
        Task EditAsync(int? id, InstagramEditDto model);
    }
}
