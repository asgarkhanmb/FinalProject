using AutoMapper;
using Domain.Entities;
using Repository.Helpers;
using Repository.Repositories.Interfaces;
using Service.DTOs.Ui.Contacts;
using Service.Helpers.Exceptions;
using Service.Services.Interfaces;

namespace Service.Services
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepo;
        private readonly IMapper _mapper;

        public ContactService(IContactRepository contactRepo,
                              IMapper mapper)
        {
            _contactRepo = contactRepo;
            _mapper = mapper;
        }


        public async Task CreateAsync(ContactCreateDto model)
        {
            if (model.Name.Length > 20||model.Message.Length>200) throw new RequiredException("Exceed the length limit!!");
            if (model == null)  throw new NotFoundException("Data not found");
            if (string.IsNullOrEmpty(model.Email) || model.Email.Length > 50 || !model.Email.Contains("@gmail.com"))
            {
                throw new RequiredException("Email is required, must be less than 50 characters, and must contain '@'.");
            }
            var contact = _mapper.Map<Contact>(model);
            await _contactRepo.CreateAsync(contact);
        }

        public async Task DeleteAsync(int? id)
        {
            ArgumentNullException.ThrowIfNull(nameof(id));
            var existContact = await _contactRepo.GetById((int)id) ?? throw new NotFoundException("Data not found");
            await _contactRepo.DeleteAsync(existContact);
        }
        public async Task<IEnumerable<ContactDto>> GetAllAsync()
        {
            return _mapper.Map<List<ContactDto>>(await _contactRepo.GetAllAsync());
        }

        public async Task<ContactDto> GetByIdAsync(int? id)
        {
            if (id is null) throw new ArgumentNullException();

            var existContact = await _contactRepo.GetById((int)id) ?? throw new NotFoundException("Data not found");

            if (existContact is null) throw new NullReferenceException();

            return _mapper.Map<ContactDto>(existContact);
        }

        public async Task<PaginationResponse<ContactDto>> GetPaginateDataAsync(int page, int take)
        {
            var contact = await _contactRepo.GetAllAsync();
            int totalPage = (int)Math.Ceiling((decimal)contact.Count() / take);

            var mappedDatas = _mapper.Map<IEnumerable<ContactDto>>(await _contactRepo.GetPaginateDataAsync(page, take));
            return new PaginationResponse<ContactDto>(mappedDatas, totalPage, page);
        }
    }
}
