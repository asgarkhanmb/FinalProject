using Domain.Entities;


namespace Repository.Repositories.Interfaces
{
    public interface IProductRepository :IBaseRepository<Product>
    {
        Task<IEnumerable<Product>> GetPaginateDataAsync(int page, int take);
        Task<IEnumerable<Product>> SortBy(string sortKey, bool isDescending);
        Task<IEnumerable<Product>> FilterAsync(string name, string categoryName,decimal? price);
    }
}
