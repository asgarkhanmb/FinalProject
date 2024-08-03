using Service.DTOs.Admin.Subscribes;

namespace Service.Services.Interfaces
{
    public interface ISubscribeService
    {
        Task AddSubscribeAsync(SubscribeCreateDto model);
        Task<IEnumerable<SubscribeDto>> GetAllAsync();

    }
}
