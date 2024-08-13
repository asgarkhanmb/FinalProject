using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Repository.Helpers;
using Repository.Repositories.Interfaces;
using Service.DTOs.Admin.Blogs;
using Service.Helpers.Exceptions;
using Service.Helpers.Extensions;
using Service.Services.Interfaces;

namespace Service.Services
{
    public class BlogService : IBlogService
    {
        private readonly IBlogRepository _blogRepo;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public BlogService(IBlogRepository blogRepo,
                           IWebHostEnvironment env,
                           IMapper mapper)
        {
            _blogRepo = blogRepo;
            _env = env;
            _mapper = mapper;
        }

        public async Task CreateAsync(BlogCreateDto model)
        {
            if (!model.UploadImage.CheckFileType("image"))
                throw new RequiredException("Invalid file type. Only image files are allowed.");

            if (!model.UploadImage.CheckFileSize(1024))
                throw new RequiredException("File size exceeds the limit.");
            if (model.Title.Length > 100 || model.Description.Length > 300) throw new RequiredException("Exceed the Title or Description length limit!!");
            string fileName = Guid.NewGuid().ToString() + "-" + model.UploadImage.FileName;

            string path = _env.GenerateFilePath("images", fileName);

            await model.UploadImage.SaveFileToLocalAsync(path);

            model.Image = fileName;

            await _blogRepo.CreateAsync(_mapper.Map<Blog>(model));
        }

        public async Task DeleteAsync(int? id)
        {
            ArgumentNullException.ThrowIfNull(nameof(id));
            var existBlog = await _blogRepo.GetById((int)id) ?? throw new NotFoundException("Data not found");
            string path = _env.GenerateFilePath("images", existBlog.Image);
            path.DeleteFileFromLocal();

            await _blogRepo.DeleteAsync(existBlog);
        }

        public async Task EditAsync(int? id, BlogEditDto model)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id), "Id cannot be null.");
            }

            if (model.Title.Length > 100 || model.Description.Length > 300)
            {
                throw new RequiredException("Exceed the Title or Description length limit!");
            }

            var existBlog = await _blogRepo.GetById((int)id) ?? throw new NotFoundException("Data not found");

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

                string oldPath = _env.GenerateFilePath("images", existBlog.Image);
                oldPath.DeleteFileFromLocal();

                string fileName = Guid.NewGuid().ToString() + "-" + model.UploadImage.FileName;
                string newPath = _env.GenerateFilePath("images", fileName);
                await model.UploadImage.SaveFileToLocalAsync(newPath);


                model.Image = fileName;
            }


            _mapper.Map(model, existBlog);
            await _blogRepo.EditAsync(existBlog);
        }

        public async Task<IEnumerable<BlogDto>> GetAllAsync()
        {
            return _mapper.Map<List<BlogDto>>(await _blogRepo.GetAllAsync());
        }

        public async Task<BlogDto> GetByIdAsync(int? id)
        {
            if (id is null) throw new ArgumentNullException();

            var existBlog = await _blogRepo.GetById((int)id) ?? throw new NotFoundException("Data not found");

            if (existBlog is null) throw new NullReferenceException();

            return _mapper.Map<BlogDto>(existBlog);
        }

        public async Task<PaginationResponse<BlogDto>> GetPaginateDataAsync(int page, int take)
        {
            var blog = await _blogRepo.GetAllAsync();
            int totalPage = (int)Math.Ceiling((decimal)blog.Count() / take);

            var mappedDatas = _mapper.Map<IEnumerable<BlogDto>>(await _blogRepo.GetPaginateDataAsync(page, take));
            return new PaginationResponse<BlogDto>(mappedDatas, totalPage, page);
        }
    }
}
