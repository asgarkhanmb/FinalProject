using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Repository.Repositories.Interfaces;
using Service.DTOs.Admin.Settings;
using Service.DTOs.Admin.Sliders;
using Service.Helpers.Exceptions;
using Service.Helpers.Extensions;
using Service.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (model.Title.Length > 20 || model.Phone.Length > 50) throw new RequiredException("Exceed the Title or Phone length limit!!");
            var existSetting = await _settingRepo.GetById((int)id) ?? throw new NotFoundException("Data not found");

            if (model.UploadImage is not null)
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
