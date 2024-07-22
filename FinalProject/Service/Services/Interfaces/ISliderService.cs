using Repository.Helpers;
using Service.DTOs.Admin.Sliders;


namespace Service.Services.Interfaces
{
    public interface ISliderService
    {
        Task<IEnumerable<SliderDto>> GetAllAsync();
        Task<SliderDto> GetByIdAsync(int? id);
        Task CreateAsync(SliderCreateDto model);
        Task DeleteAsync(int? id);
        Task EditAsync(int? id, SliderEditDto model);
        Task<PaginationResponse<SliderDto>> GetPaginateDataAsync(int page, int take);
    }
}
