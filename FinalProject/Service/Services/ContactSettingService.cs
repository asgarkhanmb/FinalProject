using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Repository.Helpers;
using Repository.Repositories.Interfaces;
using Service.DTOs.Admin.Categories;
using Service.DTOs.Admin.ContactSettings;
using Service.Helpers.Exceptions;
using Service.Helpers.Extensions;
using Service.Services.Interfaces;

namespace Service.Services
{
    public class ContactSettingService : IContactSettingService
    {
        private readonly IContactSettingRepository _contactSettingRepo;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;


        public ContactSettingService(IContactSettingRepository contactSettingRepository,
                                     IWebHostEnvironment env,
                                     IMapper mapper)
        {
            _contactSettingRepo = contactSettingRepository;
            _env = env;
            _mapper = mapper;
        }


        public async Task CreateAsync(ContactSettingCreateDto model)
        {
            if (!model.UploadImage.CheckFileType("image"))
                throw new RequiredException("Invalid file type. Only image files are allowed.");

            if (!model.UploadImage.CheckFileSize(1024))
                throw new RequiredException("File size exceeds the limit.");
            if (model.Title.Length > 20) throw new RequiredException("Exceed the Title length limit!!");
            string fileName = Guid.NewGuid().ToString() + "-" + model.UploadImage.FileName;

            string path = _env.GenerateFilePath("images", fileName);

            await model.UploadImage.SaveFileToLocalAsync(path);

            model.Image = fileName;

            await _contactSettingRepo.CreateAsync(_mapper.Map<ContactSetting>(model));
        }

        public async Task DeleteAsync(int? id)
        {
            ArgumentNullException.ThrowIfNull(nameof(id));
            var existContactSetting = await _contactSettingRepo.GetById((int)id) ?? throw new NotFoundException("Data not found");
            string path = _env.GenerateFilePath("images", existContactSetting.Image);
            path.DeleteFileFromLocal();

            await _contactSettingRepo.DeleteAsync(existContactSetting);
        }

        public async Task EditAsync(int? id, ContactSettingEditDto model)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id), "Id cannot be null.");
            }

            if (model.Title.Length > 20)
            {
                throw new RequiredException("Exceed the Title length limit!");
            }

            var existContactSetting = await _contactSettingRepo.GetById((int)id)
                ?? throw new NotFoundException("Data not found");

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


                if (!string.IsNullOrEmpty(existContactSetting.Image))
                {
                    string oldPath = _env.GenerateFilePath("images", existContactSetting.Image);
                    oldPath.DeleteFileFromLocal();
                }


                string fileName = Guid.NewGuid().ToString() + "-" + model.UploadImage.FileName;
                string newPath = _env.GenerateFilePath("images", fileName);
                await model.UploadImage.SaveFileToLocalAsync(newPath);

                model.Image = fileName;
            }

            _mapper.Map(model, existContactSetting);
            await _contactSettingRepo.EditAsync(existContactSetting);
        }

        public async Task<IEnumerable<ContactSettingDto>> GetAllAsync()
        {
            return _mapper.Map<List<ContactSettingDto>>(await _contactSettingRepo.GetAllAsync());
        }

        public async Task<ContactSettingDto> GetByIdAsync(int? id)
        {
            if (id is null) throw new ArgumentNullException();

            var existContactSetting = await _contactSettingRepo.GetById((int)id) ?? throw new NotFoundException("Data not found");

            if (existContactSetting is null) throw new NullReferenceException();

            return _mapper.Map<ContactSettingDto>(existContactSetting);
        }

    }
}
