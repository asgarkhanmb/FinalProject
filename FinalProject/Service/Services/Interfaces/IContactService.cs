using Repository.Helpers;
using Service.DTOs.Ui.Contacts;


namespace Service.Services.Interfaces
{
    public interface IContactService 
    {
        Task CreateAsync(ContactCreateDto model);
        Task<IEnumerable<ContactDto>> GetAllAsync();
        Task<ContactDto> GetByIdAsync(int? id);
        Task DeleteAsync(int? id);
        Task<PaginationResponse<ContactDto>> GetPaginateDataAsync(int page, int take);
    }
}
