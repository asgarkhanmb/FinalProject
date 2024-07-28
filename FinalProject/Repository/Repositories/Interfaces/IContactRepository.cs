using Domain.Entities;


namespace Repository.Repositories.Interfaces
{
    public interface IContactRepository :IBaseRepository<Contact>
    {
        Task<IEnumerable<Contact>> GetPaginateDataAsync(int page, int take);
    }
}
