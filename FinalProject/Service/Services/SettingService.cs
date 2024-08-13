using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Repository.Repositories.Interfaces;
using Service.DTOs.Admin.Settings;
using Service.Helpers.Exceptions;
using Service.Helpers.Extensions;
using Service.Services.Interfaces;

namespace Service.Services
{
    public class SettingService : ISettingService
    {
        private readonly ISettingRepository _settingRepo;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public SettingService(ISettingRepository settingRepo,
                              IWebHostEnvironment env,
                              IMapper mapper)
        {
            _env = env;
            _settingRepo = settingRepo;
            _mapper = mapper;
        }
        public async Task CreateAsync(SettingCreateDto model)
        {
            if (!model.UploadImage.CheckFileType("image"))
                throw new RequiredException("Invalid file type. Only image files are allowed.");

            if (!model.UploadImage.CheckFileSize(1024))
                throw new RequiredException("File size exceeds the limit.");
            bool titleExists = await _settingRepo.ExistAsync(m => m.Title == model.Title);

            if (titleExists)
            {
                throw new RequiredException("A Title already exists.");
            }
            bool phoneExists = await _settingRepo.ExistAsync(m => m.Phone == model.Phone);

            if (phoneExists)
            {
                throw new RequiredException("A Phone number already exists.");
            }

            if (model.Title.Length > 20 || model.Phone.Length > 50) throw new RequiredException("Exceed the Title or Phone length limit!!");
            string fileName = Guid.NewGuid().ToString() + "-" + model.UploadImage.FileName;

            string path = _env.GenerateFilePath("images", fileName);

            await model.UploadImage.SaveFileToLocalAsync(path);

            model.Logo = fileName;

            await _settingRepo.CreateAsync(_mapper.Map<Setting>(model));
        }

        public async Task DeleteAsync(int? id)
        {
            ArgumentNullException.ThrowIfNull(nameof(id));
            var existSetting = await _settingRepo.GetById((int)id) ?? throw new NotFoundException("Data not found");
            string path = _env.GenerateFilePath("images", existSetting.Logo);
            path.DeleteFileFromLocal();

            await _settingRepo.DeleteAsync(existSetting);
        }

        public async Task EditAsync(int? id, SettingEditDto model)
        {

            if (id == null)
            {
                throw new ArgumentNullException(nameof(id), "Id cannot be null.");
            }

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
            }

            bool titleExists = await _settingRepo.ExistAsync(m => m.Title == model.Title);
            if (titleExists)
            {
                throw new RequiredException("A Title already exists.");
            }

            bool phoneExists = await _settingRepo.ExistAsync(m => m.Phone == model.Phone);
            if (phoneExists)
            {
                throw new RequiredException("A Phone number already exists.");
            }

            if (model.Title.Length > 20 || model.Phone.Length > 50)
            {
                throw new RequiredException("Exceed the Title or Phone length limit!!");
            }

            var existSetting = await _settingRepo.GetById((int)id) ?? throw new NotFoundException("Data not found");

            if (model.UploadImage != null)
            {
                string oldPath = _env.GenerateFilePath("images", existSetting.Logo);
                oldPath.DeleteFileFromLocal();

                string fileName = Guid.NewGuid().ToString() + "-" + model.UploadImage.FileName;
                string newPath = _env.GenerateFilePath("images", fileName);

                await model.UploadImage.SaveFileToLocalAsync(newPath);

                model.Logo = fileName;
            }

            _mapper.Map(model, existSetting);
            await _settingRepo.EditAsync(existSetting);

        }

        public async Task<IEnumerable<SettingDto>> GetAllAsync()
        {
            return _mapper.Map<List<SettingDto>>(await _settingRepo.GetAllAsync());
        }

        public async Task<SettingDto> GetByIdAsync(int? id)
        {
            if (id is null) throw new ArgumentNullException();

            var existSetting = await _settingRepo.GetById((int)id) ?? throw new NotFoundException("Data not found");

            if (existSetting is null) throw new NullReferenceException();

            return _mapper.Map<SettingDto>(existSetting);
        }
    }
}
