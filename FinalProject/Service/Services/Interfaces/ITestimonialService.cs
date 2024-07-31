using Repository.Helpers;
using Service.DTOs.Admin.Testimonials;


namespace Service.Services.Interfaces
{
    public interface ITestimonialService
    {
        Task<IEnumerable<TestimonialDto>> GetAllAsync();
        Task<TestimonialDto> GetByIdAsync(int? id);
        Task CreateAsync(TestimonialCreateDto model);
        Task DeleteAsync(int? id);
        Task EditAsync(int? id, TestimonialEditDto model);
        Task<PaginationResponse<TestimonialDto>> GetPaginateDataAsync(int page, int take);
    }
}
