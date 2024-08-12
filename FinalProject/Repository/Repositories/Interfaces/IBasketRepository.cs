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
        Task AddAsync(Basket basket);
        Task SaveChangesAsync();
        void Remove(BasketProduct basketProduct);
    }
}
