using Domain.Entities;
using Repository.Data;
using Repository.Repositories.Interfaces;

namespace Repository.Repositories
{
    public class ContactSettingRepository :BaseRepository<ContactSetting>,IContactSettingRepository
    {
        public ContactSettingRepository(AppDbContext context) : base(context) { }

    }
}
