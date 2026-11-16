using AutoMapper;
using Domain.Entities;
using Repository.Repositories.Interfaces;
using Service.DTOs.Ui.Subscribes;
using Service.Helpers.Exceptions;
using Service.Services.Interfaces;


namespace Service.Services
{
    public class SubscribeService : ISubscribeService
    {
        private readonly ISubscribeRepository _subscribeRepo;
        private readonly IMapper _mapper;

        public SubscribeService(ISubscribeRepository subscribeRepo,
                                 IMapper mapper)
        {
            _mapper = mapper;
            _subscribeRepo = subscribeRepo;
        }

        public async Task AddSubscribeAsync(SubscribeCreateDto model)
        {
            bool subscribeExists = await _subscribeRepo.ExistAsync(m => m.Email == model.Email);

            if (subscribeExists)
            {
                throw new RequiredException("Bu e-poçt artıq abunə olub.");
            }
            if (model == null) throw new NotFoundException("Data not found");
            if (string.IsNullOrEmpty(model.Email) || model.Email.Length > 200 || model.Email.Length <15)
            {
                throw new RequiredException("E-poçt tələb olunur (5–200 simvol).");
            }
            var subscribe = _mapper.Map<Subscribe>(model);
            await _subscribeRepo.CreateAsync(subscribe);
        }

        public async Task<IEnumerable<SubscribeDto>> GetAllAsync()
        {
            return _mapper.Map<List<SubscribeDto>>(await _subscribeRepo.GetAllAsync());
        }
    }
}
