using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Repository.Helpers;
using Repository.Repositories.Interfaces;
using Service.DTOs.Admin.Sliders;
using Service.Helpers.Exceptions;
using Service.Helpers.Extensions;
using Service.Services.Interfaces;

namespace Service.Services
{
    public class SliderService : ISliderService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ISliderRepository _sliderRepo;
        private readonly IMapper _mapper;


        public SliderService(IWebHostEnvironment env,
                             ISliderRepository sliderRepo,
                             IMapper mapper)
        {
            _env = env;
            _sliderRepo = sliderRepo;
            _mapper = mapper;
        }

        public async Task CreateAsync(SliderCreateDto model)
        {
            string fileName = Guid.NewGuid().ToString() + "-" + model.UploadImage.FileName;

            string path = _env.GenerateFilePath( "images", fileName);

            await model.UploadImage.SaveFileToLocalAsync(path);

            model.Image = fileName;

            await _sliderRepo.CreateAsync(_mapper.Map<Slider>(model));
        }

        public async Task DeleteAsync(int? id)
        {
            ArgumentNullException.ThrowIfNull(nameof(id));
            var existSlider = await _sliderRepo.GetById((int)id) ?? throw new NotFoundException("Data not found");
            string path = _env.GenerateFilePath("images", existSlider.Image);
            path.DeleteFileFromLocal();

           await _sliderRepo.DeleteAsync(existSlider);
        }

        public async Task EditAsync(int? id, SliderEditDto model)
        {
            var existSlider = await _sliderRepo.GetById((int)id) ?? throw new NotFoundException("Data not found");

            if (model.UploadImage is not null)
            {
                string oldPath = _env.GenerateFilePath("images", existSlider.Image);
                oldPath.DeleteFileFromLocal();

                string fileName = Guid.NewGuid().ToString() + "-" + model.UploadImage.FileName;

                string newPath = _env.GenerateFilePath( "images", fileName);

                await model.UploadImage.SaveFileToLocalAsync(newPath);

                model.Image = fileName;
             
            }
            _mapper.Map(model, existSlider);

          

            await _sliderRepo.EditAsync(existSlider);
        }

        public async Task<IEnumerable<SliderDto>> GetAllAsync()
        {
            return _mapper.Map<List<SliderDto>>(await _sliderRepo.GetAllAsync());
        }

        public async Task<SliderDto> GetByIdAsync(int? id)
        {
            if (id is null) throw new ArgumentNullException();

            var existSlider = await _sliderRepo.GetById((int)id) ?? throw new NotFoundException("Data not found");

            if (existSlider is null) throw new NullReferenceException();

            return _mapper.Map<SliderDto>(existSlider);

        }

        public async Task<PaginationResponse<SliderDto>> GetPaginateDataAsync(int page, int take)
        {
            var cities = await _sliderRepo.GetAllAsync();
            int totalPage = (int)Math.Ceiling((decimal)cities.Count() / take);

            var mappedDatas = _mapper.Map<IEnumerable<SliderDto>>(await _sliderRepo.GetPaginateDataAsync(page, take));
            return new PaginationResponse<SliderDto>(mappedDatas, totalPage, page);
        }
    }
}
