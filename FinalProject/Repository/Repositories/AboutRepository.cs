using Domain.Entities;
using Repository.Data;
using Repository.Repositories.Interfaces;


namespace Repository.Repositories
{
    public class AboutRepository :BaseRepository<About>,IAboutRepository
    {
        public AboutRepository(AppDbContext context):base(context) { }
        
    }
}
