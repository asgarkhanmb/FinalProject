using Service.DTOs.Ui.Subscribes;

namespace Service.Services.Interfaces
{
    public interface ISubscribeService
    {
        Task AddSubscribeAsync(SubscribeCreateDto model);
        Task<IEnumerable<SubscribeDto>> GetAllAsync();

    }
}
