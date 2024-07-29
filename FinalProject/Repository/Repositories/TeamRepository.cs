using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Repositories.Interfaces;

namespace Repository.Repositories
{
    public class TeamRepository :BaseRepository<Team>,ITeamRepository
    {
        public TeamRepository(AppDbContext context):base(context) { }


        public async Task<IEnumerable<Team>> GetPaginateDataAsync(int page, int take)
        {
            return await _entities.Skip((page - 1) * take).Take(take).ToListAsync();
        }
    }
}
