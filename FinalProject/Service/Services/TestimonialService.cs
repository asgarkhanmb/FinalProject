using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Repository.Helpers;
using Repository.Repositories.Interfaces;
using Service.DTOs.Admin.Testimonials;
using Service.Helpers.Exceptions;
using Service.Helpers.Extensions;
using Service.Services.Interfaces;

namespace Service.Services
{
    public class TestimonialService : ITestimonialService
    {
        private readonly ITestimonialRepository _testimonialRepo;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public TestimonialService(ITestimonialRepository testimonialRepo,
                                  IWebHostEnvironment env,
                                  IMapper mapper)
        {
            _env = env;
            _mapper = mapper;
            _testimonialRepo = testimonialRepo;
        }


        public async Task CreateAsync(TestimonialCreateDto model)
        {
            if (model.Title.Length > 30 || model.Description.Length > 300 || model.FullName.Length > 50 || model.City.Length > 50)
                throw new RequiredException("Exceed the length limit!!");

            string fileName = Guid.NewGuid().ToString() + "-" + model.UploadImage.FileName;

            string path = _env.GenerateFilePath("images", fileName);

            await model.UploadImage.SaveFileToLocalAsync(path);

            model.Image = fileName;

            await _testimonialRepo.CreateAsync(_mapper.Map<Testimonial>(model));
        }

        public async Task DeleteAsync(int? id)
        {
            ArgumentNullException.ThrowIfNull(nameof(id));
            var existTestimonial = await _testimonialRepo.GetById((int)id) ?? throw new NotFoundException("Data not found");
            string path = _env.GenerateFilePath("images", existTestimonial.Image);
            path.DeleteFileFromLocal();

            await _testimonialRepo.DeleteAsync(existTestimonial);
        }

        public async Task EditAsync(int? id, TestimonialEditDto model)
        {
            var existTestimonial = await _testimonialRepo.GetById((int)id) ?? throw new NotFoundException("Data not found");

            if (model.UploadImage is not null)
            {
                string oldPath = _env.GenerateFilePath("images", existTestimonial.Image);
                oldPath.DeleteFileFromLocal();

                string fileName = Guid.NewGuid().ToString() + "-" + model.UploadImage.FileName;

                string newPath = _env.GenerateFilePath("images", fileName);

                await model.UploadImage.SaveFileToLocalAsync(newPath);

                model.Image = fileName;

            }
            _mapper.Map(model, existTestimonial);
            await _testimonialRepo.EditAsync(existTestimonial);
        }

        public async Task<IEnumerable<TestimonialDto>> GetAllAsync()
        {
            return _mapper.Map<List<TestimonialDto>>(await _testimonialRepo.GetAllAsync());
        }

        public async Task<TestimonialDto> GetByIdAsync(int? id)
        {
            if (id is null) throw new ArgumentNullException();

            var existTestimonial = await _testimonialRepo.GetById((int)id) ?? throw new NotFoundException("Data not found");

            if (existTestimonial is null) throw new NullReferenceException();

            return _mapper.Map<TestimonialDto>(existTestimonial);
        }

        public async Task<PaginationResponse<TestimonialDto>> GetPaginateDataAsync(int page, int take)
        {
            var testimonials = await _testimonialRepo.GetAllAsync();
            int totalPage = (int)Math.Ceiling((decimal)testimonials.Count() / take);

            var mappedDatas = _mapper.Map<IEnumerable<TestimonialDto>>(await _testimonialRepo.GetPaginateDataAsync(page, take));
            return new PaginationResponse<TestimonialDto>(mappedDatas, totalPage, page);
        }
    }
}
