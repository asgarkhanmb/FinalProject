﻿using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Repositories.Interfaces;

namespace Repository.Repositories
{
    public class SliderRepository :BaseRepository<Slider>,ISliderRepository
    {
        public SliderRepository(AppDbContext context):base(context) { }


        public async Task<IEnumerable<Slider>> GetPaginateDataAsync(int page, int take)
        {
            return await _entities.Skip((page - 1) * take).Take(take).ToListAsync();
        }
    }
}
