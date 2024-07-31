using Domain.Entities;


namespace Repository.Repositories.Interfaces
{
    public interface ICategoryRepository :IBaseRepository<Category>
    {
        Task<IEnumerable<Category>> GetPaginateDataAsync(int page, int take);
        
    }
}
