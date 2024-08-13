using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Repository.Helpers;
using Repository.Repositories.Interfaces;
using Service.DTOs.Admin.Products;
using Service.Helpers.Exceptions;
using Service.Helpers.Extensions;
using Service.Services.Interfaces;

namespace Service.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepo,
                              ICategoryRepository categoryRepo,
                              IWebHostEnvironment env,
                              IMapper mapper)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
            _env = env;
            _mapper = mapper;
        }


        public async Task CreateAsync(ProductCreateDto model)
        {
            foreach (var item in model.UploadImages)
            {
                if (!item.CheckFileType("image"))
                    throw new RequiredException("Invalid file type. Only image files are allowed.");

                if (!item.CheckFileSize(1024))
                    throw new RequiredException("File size exceeds the limit.");
            }
            bool productExists = await _productRepo.ExistAsync(m=>m.Name==model.Name);
            if (model.Name.Length > 50)
            {
                throw new RequiredException("Exceed the Name length limit!!");
            }
            if (productExists)
            {
                throw new RequiredException("A product with the same name already exists.");
            }

            bool categoryExists = await _categoryRepo.ExistAsync(c => c.Id == model.CategoryId);

            if (!categoryExists)
            {
                throw new NotFoundException("The specified category does not exist.");
            }


            List<ProductImage> images = new();

            foreach (var item in model.UploadImages)
            {
                string fileName = $"{Guid.NewGuid()}-{item.FileName}";

                string path = _env.GenerateFilePath("images", fileName);

                await item.SaveFileToLocalAsync(path);

                images.Add(new ProductImage { Image = fileName });
            }

            images.FirstOrDefault().IsMain = true;
            model.ProductImages = images;

            await _productRepo.CreateAsync(_mapper.Map<Product>(model));
        }

        public async Task DeleteAsync(int? id)
        {
            var existProduct = await _productRepo.GetById((int)id) ?? throw new NotFoundException("Data not found");
            var product = await _productRepo.FindAllWithIncludes()
            .Where(m => m.Id == id)
            .Include(product => product.ProductImages)
            .AsNoTracking()
            .FirstOrDefaultAsync();

            foreach (var item in product.ProductImages)
            {
                string path = _env.GenerateFilePath("images", item.Image);
                path.DeleteFileFromLocal();
            }


            await _productRepo.DeleteAsync(product);
        }

        public async Task EditAsync(int? id, ProductEditDto model)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id), "Product ID cannot be null.");
            }
            if (model.Name.Length > 50)
            {
                throw new RequiredException("Exceed the Name length limit!!");
            }
            if (model.UploadImages != null)
            {
                foreach (var item in model.UploadImages)
                {
                    if (!item.CheckFileType("image"))
                    {
                        throw new RequiredException("Invalid file type. Only image files are allowed.");
                    }

                    if (!item.CheckFileSize(1024))
                    {
                        throw new RequiredException("File size exceeds the 1MB limit.");
                    }
                }
            }
            bool productExists = await _productRepo.ExistAsync(m => m.Name == model.Name);

            if (productExists)
            {
                throw new RequiredException("A product with the same name already exists.");
            }
            var existProduct = await _productRepo.GetByInclude(p=>p.Id==id, "ProductImages");
            _mapper.Map(model, existProduct);

            if (model.UploadImages is not null)
            {
                   
                List<string> oldImagePaths = existProduct.ProductImages.Select(p => p.Image).ToList();

                foreach (var oldPathImage in oldImagePaths)
                {
                    if (File.Exists(Path.Combine(_env.WebRootPath, "images", oldPathImage)))
                        File.Delete(Path.Combine(_env.WebRootPath, "images", oldPathImage));

                }
                int i = 0;
                existProduct.ProductImages = new();
                foreach (var item in model.UploadImages)
                {

                    string fileName = Guid.NewGuid().ToString() + "-" + item.FileName;

                    string newPath = _env.GenerateFilePath("images", fileName);

                    await item.SaveFileToLocalAsync(newPath);

                    existProduct.ProductImages.Add(new ProductImage { Image = fileName, IsMain =++i==1? true:false });
                }
            }

            await _productRepo.EditAsync(existProduct);
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            return _mapper.Map<IEnumerable<ProductDto>>(await _productRepo.FindAllWithIncludes()
              .Include(m => m.ProductImages)
              .Include(m => m.Category)
              .AsNoTracking()
              .ToListAsync());
        }

        public async Task<ProductDto> GetByIdAsync(int? id)
        {
            var existProduct = await _productRepo.GetById((int)id) ?? throw new NotFoundException("Data not found");
            return _mapper.Map<ProductDto>(await _productRepo.GetByInclude(p=>p.Id==id,"ProductImages","Category"));
        }

        public async Task<PaginationResponse<ProductDto>> GetPaginateDataAsync(int page, int take)
        {
            var product = await _productRepo.FindAllWithIncludes().Include(m=>m.ProductImages).Include(m=>m.Category).ToListAsync();
            int totalPage = (int)Math.Ceiling((decimal)product.Count() / take);

            var mappedDatas = _mapper.Map<IEnumerable<ProductDto>>(await _productRepo.GetPaginateDataAsync(page, take));
            return new PaginationResponse<ProductDto>(mappedDatas, totalPage, page);
        }

        public async Task<IEnumerable<ProductDto>> Search(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new NotFoundException("Data not found");
            var product = await _productRepo.FindAllWithIncludes().Include(m => m.Category).Include(m => m.ProductImages).ToListAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(await _productRepo.FindAll(m => m.Name.Contains(name)));
        }

        public async Task<IEnumerable<ProductDto>> SortBy(string sortKey, bool isDescending)
        {
            var validSortKeys = new List<string> { "Name","name", "Price","price" };
            if (string.IsNullOrEmpty(sortKey) || !validSortKeys.Contains(sortKey))
            {
                throw new NotFoundException("Invalid sortKey");
            }
            var product = await _productRepo.FindAllWithIncludes().Include(m => m.Category).Include(m => m.ProductImages).ToListAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(await _productRepo.SortBy(sortKey, isDescending));
        }
        public async Task<IEnumerable<ProductDto>> FilterAsync(string categoryName)
        {
            return _mapper.Map<IEnumerable<ProductDto>>(await _productRepo.FilterAsync(categoryName));
        }

    }
}
