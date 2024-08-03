using Service.DTOs.Admin.Settings;

namespace Service.Services.Interfaces
{
    public interface ISettingService
    {
        Task<IEnumerable<SettingDto>> GetAllAsync();
        Task<SettingDto> GetByIdAsync(int? id);
        Task CreateAsync(SettingCreateDto model);
        Task DeleteAsync(int? id);
        Task EditAsync(int? id, SettingEditDto model);
    }
}
