using Repository.Helpers;
using Service.DTOs.Admin.ContactSettings;


namespace Service.Services.Interfaces
{
    public interface IContactSettingService
    {
        Task<IEnumerable<ContactSettingDto>> GetAllAsync();
        Task<ContactSettingDto> GetByIdAsync(int? id);
        Task CreateAsync(ContactSettingCreateDto model);
        Task DeleteAsync(int? id);
        Task EditAsync(int? id, ContactSettingEditDto model);
    }
}
