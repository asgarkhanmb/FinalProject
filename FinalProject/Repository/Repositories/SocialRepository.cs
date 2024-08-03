using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Repositories.Interfaces;

namespace Repository.Repositories
{
    public class SocialRepository :BaseRepository<Social>,ISocialRepository
    {
        public SocialRepository(AppDbContext context) : base(context) { }


        public async Task<IEnumerable<Social>> GetPaginateDataAsync(int page, int take)
        {
            return await _entities.Skip((page - 1) * take).Take(take).ToListAsync();
        }

    }
}
