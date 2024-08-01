using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Repository.Repositories.Interfaces;
using Service.DTOs.Admin.Instagrams;
using Service.DTOs.Admin.Products;
using Service.Helpers.Exceptions;
using Service.Helpers.Extensions;
using Service.Services.Interfaces;


namespace Service.Services
{
    public class InstagramService : IInstagramService
    {
        private readonly IInstagramRepository _instagramRepo;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public InstagramService(IInstagramRepository instagramRepo,
                                IWebHostEnvironment env,
                                IMapper mapper)
        {
            _env = env;
            _instagramRepo = instagramRepo;
            _mapper = mapper;
        }
        public async Task CreateAsync(InstagramCreateDto model)
        {
            bool instaExists = await _instagramRepo.ExistAsync(m => m.SocialName == model.SocialName);

            if (instaExists)
            {
                throw new RequiredException("A SocialName with the same name already exists.");
            }

            List<InstagramGallery> images = new();

            foreach (var item in model.UploadImages)
            {
                string fileName = $"{Guid.NewGuid()}-{item.FileName}";

                string path = _env.GenerateFilePath("images", fileName);

                await item.SaveFileToLocalAsync(path);

                images.Add(new InstagramGallery { Image = fileName });
            }

          
            model.InstagramGalleries = images;

            await _instagramRepo.CreateAsync(_mapper.Map<Instagram>(model));
        }

        public async Task DeleteAsync(int? id)
        {
            ArgumentNullException.ThrowIfNull(nameof(id));
            var existCategory = await _instagramRepo.GetById((int)id) ?? throw new NotFoundException("Data not found");
            string path = _env.GenerateFilePath("images", existCategory.InstagramGalleries.ToString());
            path.DeleteFileFromLocal();

            await _instagramRepo.DeleteAsync(existCategory);
        }

        public async Task EditAsync(int? id, InstagramEditDto model)
        {
            bool instaExists = await _instagramRepo.ExistAsync(m => m.SocialName == model.SocialName);

            if (instaExists)
            {
                throw new RequiredException("A SocialName with the same name already exists.");
            }
            var existInsta = await _instagramRepo.GetByInclude(p => p.Id == id, "ProductImages");
            _mapper.Map(model, existInsta);

            if (model.UploadImages is not null)
            {

                List<string> oldImagePaths = existInsta.InstagramGalleries.Select(p => p.Image).ToList();

                foreach (var oldPathImage in oldImagePaths)
                {
                    if (File.Exists(Path.Combine(_env.WebRootPath, "images", oldPathImage)))
                        File.Delete(Path.Combine(_env.WebRootPath, "images", oldPathImage));

                }
                existInsta.InstagramGalleries= new();
                foreach (var item in model.UploadImages)
                {

                    string fileName = Guid.NewGuid().ToString() + "-" + item.FileName;

                    string newPath = _env.GenerateFilePath("images", fileName);

                    await item.SaveFileToLocalAsync(newPath);

                    existInsta.InstagramGalleries.Add(new InstagramGallery { Image = fileName});
                }
            }

            await _instagramRepo.EditAsync(existInsta);
        }

        public async Task<IEnumerable<InstagramDto>> GetAllAsync()
        {
            return _mapper.Map<List<InstagramDto>>(await _instagramRepo.FindAllWithIncludes()
                .Include(m=>m.InstagramGalleries).ToListAsync());
        }

        public async Task<InstagramDto> GetByIdAsync(int? id)
        {
            var existInsta = await _instagramRepo.GetById((int)id) ?? throw new NotFoundException("Data not found");
            return _mapper.Map<InstagramDto>(await _instagramRepo.GetByInclude(p => p.Id == id, "InstagramGalleries"));
        }
    }
}
