using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Repository.Helpers;
using Repository.Repositories.Interfaces;
using Service.DTOs.Admin.Socials;
using Service.Helpers.Exceptions;
using Service.Services.Interfaces;

namespace Service.Services
{
    public class SocialService : ISocialService
    {
        private readonly ISocialRepository _socialRepo;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;


        public SocialService(ISocialRepository socialRepo,
                             IWebHostEnvironment env,
                             IMapper mapper)
        {
            _socialRepo = socialRepo;
            _env = env;
            _mapper = mapper;
        }


        public async Task CreateAsync(SocialCreateDto model)
        {
            if (model.Name.Length > 50)
            {
                throw new RequiredException("Exceed the Name length limit!!");
            }
            bool socialExists = await _socialRepo.ExistAsync(m => m.Url == model.Url);

            if (socialExists)
            {
                throw new RequiredException("A social url with the same name already exists.");
            }
            var contact = _mapper.Map<Social>(model);
            await _socialRepo.CreateAsync(contact);
        }

        public async Task DeleteAsync(int? id)
        {
            ArgumentNullException.ThrowIfNull(nameof(id));
            var existSocial = await _socialRepo.GetById((int)id) ?? throw new NotFoundException("Data not found");
            await _socialRepo.DeleteAsync(existSocial);
        }

        public async Task EditAsync(int? id, SocialEditDto model)
        {
            if (model.Name.Length > 50)
            {
                throw new RequiredException("Exceed the Name length limit!!");
            }
            bool socialExists = await _socialRepo.ExistAsync(m => m.Url == model.Url);

            if (socialExists)
            {
                throw new RequiredException("A social url with the same name already exists.");
            }
            var existSocial = await _socialRepo.GetById((int)id) ?? throw new NotFoundException("Data not found");

            _mapper.Map(model, existSocial);

            await _socialRepo.EditAsync(existSocial);
        }

        public async Task<IEnumerable<SocialDto>> GetAllAsync()
        {
            return _mapper.Map<List<SocialDto>>(await _socialRepo.GetAllAsync());
        }

        public async Task<SocialDto> GetByIdAsync(int? id)
        {
            var existSocial = await _socialRepo.GetById((int)id) ?? throw new NotFoundException("Data not found");
            return _mapper.Map<SocialDto>(await _socialRepo.GetByInclude(p => p.Id == id, "Url"));
        }

        public async Task<PaginationResponse<SocialDto>> GetPaginateDataAsync(int page, int take)
        {
            var social = await _socialRepo.GetAllAsync();
            int totalPage = (int)Math.Ceiling((decimal)social.Count() / take);

            var mappedDatas = _mapper.Map<IEnumerable<SocialDto>>(await _socialRepo.GetPaginateDataAsync(page, take));
            return new PaginationResponse<SocialDto>(mappedDatas, totalPage, page);
        }
    }
}
