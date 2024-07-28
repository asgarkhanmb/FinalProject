using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Repositories.Interfaces;

namespace Repository.Repositories
{
    public class ContactRepository : BaseRepository<Contact>, IContactRepository    
    {
        public ContactRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Contact>> GetPaginateDataAsync(int page, int take)
        {
            return await _entities.Skip((page - 1) * take).Take(take).ToListAsync();
        }
    }
}
