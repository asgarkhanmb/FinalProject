using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Repository.Helpers;
using Repository.Repositories.Interfaces;
using Service.DTOs.Admin.Categories;
using Service.DTOs.Admin.Products;
using Service.Helpers.Exceptions;
using Service.Helpers.Extensions;
using Service.Services.Interfaces;


namespace Service.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepo;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepo,
                               IWebHostEnvironment env,
                               IMapper mapper)
        {
            _categoryRepo = categoryRepo;
            _env = env;
            _mapper = mapper;
        }

        public async Task CreateAsync(CategoryCreateDto model)
        {
            if (!model.UploadImage.CheckFileType("image"))
                throw new RequiredException("Invalid file type. Only image files are allowed.");

            if (!model.UploadImage.CheckFileSize(1024))
                throw new RequiredException("File size exceeds the limit.");
            bool categoryExists = await _categoryRepo.ExistAsync(m=>m.Name==model.Name);

            if (categoryExists)
            {
                throw new RequiredException("A category with the same name already exists.");
            }

            if (model.Name.Length>20) throw new RequiredException("Exceed the Name length limit!!");
            string fileName = Guid.NewGuid().ToString() + "-" + model.UploadImage.FileName;

            string path = _env.GenerateFilePath("images", fileName);

            await model.UploadImage.SaveFileToLocalAsync(path);

            model.Icon = fileName;

            await _categoryRepo.CreateAsync(_mapper.Map<Category>(model));
        }

        public async Task DeleteAsync(int? id)
        {
            ArgumentNullException.ThrowIfNull(nameof(id));
            var existCategory = await _categoryRepo.GetById((int)id) ?? throw new NotFoundException("Data not found");
            string path = _env.GenerateFilePath("images", existCategory.Icon);
            path.DeleteFileFromLocal();

            await _categoryRepo.DeleteAsync(existCategory);
        }

        public async Task EditAsync(int? id, CategoryEditDto model)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id), "Id cannot be null.");
            }

            if (model.Name.Length > 20)
            {
                throw new RequiredException("Exceed the Name length limit!");
            }


            bool categoryExists = await _categoryRepo.ExistAsync(m => m.Name == model.Name && m.Id != id);

            if (categoryExists)
            {
                throw new RequiredException("A category with the same name already exists.");
            }

            var existCategory = await _categoryRepo.GetById((int)id) ?? throw new NotFoundException("Data not found");

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


                string oldPath = _env.GenerateFilePath("images", existCategory.Icon);
                oldPath.DeleteFileFromLocal();


                string fileName = Guid.NewGuid().ToString() + "-" + model.UploadImage.FileName;
                string newPath = _env.GenerateFilePath("images", fileName);
                await model.UploadImage.SaveFileToLocalAsync(newPath);

                model.Icon = fileName;
            }

            _mapper.Map(model, existCategory);
            await _categoryRepo.EditAsync(existCategory);
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            return _mapper.Map<List<CategoryDto>>(await _categoryRepo.GetAllAsync());
        }

        public async Task<CategoryDto> GetByIdAsync(int? id)
        {
            if (id is null) throw new ArgumentNullException();

            var existCategory = await _categoryRepo.GetById((int)id) ?? throw new NotFoundException("Data not found");

            if (existCategory is null) throw new NullReferenceException();

            return _mapper.Map<CategoryDto>(existCategory);
        }

        public async Task<PaginationResponse<CategoryDto>> GetPaginateDataAsync(int page, int take)
        {
            var category = await _categoryRepo.GetAllAsync();
            int totalPage = (int)Math.Ceiling((decimal)category.Count() / take);

            var mappedDatas = _mapper.Map<IEnumerable<CategoryDto>>(await _categoryRepo.GetPaginateDataAsync(page, take));
            return new PaginationResponse<CategoryDto>(mappedDatas, totalPage, page);
        }

        public async Task<IEnumerable<CategoryDto>> Search(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new NotFoundException("Data not found");
            return _mapper.Map<IEnumerable<CategoryDto>>(await _categoryRepo.FindAll(m => m.Name.Contains(name)));
        }
    }
}
