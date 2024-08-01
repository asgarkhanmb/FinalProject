using Domain.Entities;
using Repository.Data;
using Repository.Repositories.Interfaces;

namespace Repository.Repositories
{
    public class InstagramRepository : BaseRepository<Instagram>, IInstagramRepository
    {
        public InstagramRepository(AppDbContext context) : base(context) { }
    }
}
