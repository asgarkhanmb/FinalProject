using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Repository.Repositories.Interfaces;
using Service.DTOs.Admin.Abouts;
using Service.Helpers.Exceptions;
using Service.Helpers.Extensions;
using Service.Services.Interfaces;


namespace Service.Services
{
    public class AboutService : IAboutService
    {

        private readonly IAboutRepository _aboutRepo;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public AboutService(IAboutRepository aboutRepo,
                            IWebHostEnvironment env,
                            IMapper mapper)
        {
            _aboutRepo = aboutRepo;
            _env = env;
            _mapper = mapper;
        }
        public async Task CreateAsync(AboutCreateDto model)
        {
            if (!model.UploadImage.CheckFileType("image"))
                throw new RequiredException("Invalid file type. Only image files are allowed.");

            if (!model.UploadImage.CheckFileSize(1024))
                throw new RequiredException("File size exceeds the limit.");
            if (model.Title.Length > 50 || model.Description.Length>200 ) throw new RequiredException("Exceed the Title or Description length limit!!");
            
            string fileName = Guid.NewGuid().ToString() + "-" + model.UploadImage.FileName;

            string path = _env.GenerateFilePath("images", fileName);

            await model.UploadImage.SaveFileToLocalAsync(path);

            model.Image = fileName;

            await _aboutRepo.CreateAsync(_mapper.Map<About>(model));
        }

        public async Task DeleteAsync(int? id)
        {
            ArgumentNullException.ThrowIfNull(nameof(id));
            var existAbout = await _aboutRepo.GetById((int)id) ?? throw new NotFoundException("Data not found");
            string path = _env.GenerateFilePath("images", existAbout.Image);
            path.DeleteFileFromLocal();

            await _aboutRepo.DeleteAsync(existAbout);
        }

        public async Task EditAsync(int? id, AboutEditDto model)
        {
            if (id == null)
            {
                throw new NotFoundException("Id cannot be null.");
            }

            if (model.Title.Length > 50 || model.Description.Length > 200)
            {
                throw new RequiredException("Exceed the Title or Description length limit!!");
            }

            var existAbout = await _aboutRepo.GetById((int)id) ?? throw new NotFoundException("Data not found");

            if (model.UploadImage != null)
            {
                if (!model.UploadImage.CheckFileType("image"))
                {
                    throw new RequiredException("Invalid file type. Only image files are allowed.");
                }

                if (!model.UploadImage.CheckFileSize(1024))
                {
                    throw new RequiredException("File size exceeds the limit.");
                }


                string oldPath = _env.GenerateFilePath("images", existAbout.Image);
                oldPath.DeleteFileFromLocal();


                string fileName = Guid.NewGuid().ToString() + "-" + model.UploadImage.FileName;
                string newPath = _env.GenerateFilePath("images", fileName);
                await model.UploadImage.SaveFileToLocalAsync(newPath);


                model.Image = fileName;
            }
            _mapper.Map(model, existAbout);
            await _aboutRepo.EditAsync(existAbout);
        }

        public async Task<IEnumerable<AboutDto>> GetAllAsync()
        {
            return _mapper.Map<List<AboutDto>>(await _aboutRepo.GetAllAsync());
        }

        public async Task<AboutDto> GetByIdAsync(int? id)
        {
            if (id is null) throw new ArgumentNullException();

            var existAbout = await _aboutRepo.GetById((int)id) ?? throw new NotFoundException("Data not found");

            if (existAbout is null) throw new NullReferenceException();

            return _mapper.Map<AboutDto>(existAbout);
        }
    }
}
