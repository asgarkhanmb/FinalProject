using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Interfaces
{
    public interface ITeamRepository :IBaseRepository<Team>
    {
        Task<IEnumerable<Team>> GetPaginateDataAsync(int page, int take);
    }
}
