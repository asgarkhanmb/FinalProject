using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Interfaces
{
    public interface IBasketRepository :IBaseRepository<Basket>
    {
        Task<Basket> GetByUserIdAsync(string userId);
        Task<Basket> GetByIdAsync(int id);
        Task AddAsync(Basket basket);
        Task UpdateAsync(Basket basket);
        Task DeleteAsync(int id);
    }
}
