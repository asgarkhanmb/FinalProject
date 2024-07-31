using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Interfaces
{
    public interface ITestimonialRepository : IBaseRepository<Testimonial>
    {
        Task<IEnumerable<Testimonial>> GetPaginateDataAsync(int page, int take);
    }
}
