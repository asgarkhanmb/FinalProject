using Service.DTOs.Admin.Abouts;


namespace Service.Services.Interfaces
{
    public interface IAboutService
    {
        Task<IEnumerable<AboutDto>> GetAllAsync();
        Task<AboutDto> GetByIdAsync(int? id);
        Task CreateAsync(AboutCreateDto model);
        Task DeleteAsync(int? id);
        Task EditAsync(int? id, AboutEditDto model);
    }
}
