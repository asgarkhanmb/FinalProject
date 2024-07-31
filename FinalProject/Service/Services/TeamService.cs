using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Repository.Helpers;
using Repository.Repositories.Interfaces;
using Service.DTOs.Admin.Teams;
using Service.Helpers.Exceptions;
using Service.Helpers.Extensions;
using Service.Services.Interfaces;


namespace Service.Services
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepo;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public TeamService(ITeamRepository teamRepo,
                           IWebHostEnvironment env,
                           IMapper mapper)
        {
            _teamRepo = teamRepo;
            _env = env;
            _mapper = mapper;
        }


        public async Task CreateAsync(TeamCreateDto model)
        {
            if (model.Title.Length > 20 || model.FullName.Length > 50 || model.Position.Length>30) throw new RequiredException("Exceed the length limit!!");
            string fileName = Guid.NewGuid().ToString() + "-" + model.UploadImage.FileName;

            string path = _env.GenerateFilePath("images", fileName);

            await model.UploadImage.SaveFileToLocalAsync(path);

            model.Image = fileName;

            await _teamRepo.CreateAsync(_mapper.Map<Team>(model));
        }

        public async Task DeleteAsync(int? id)
        {
            ArgumentNullException.ThrowIfNull(nameof(id));
            var existTeam= await _teamRepo.GetById((int)id) ?? throw new NotFoundException("Data not found");
            string path = _env.GenerateFilePath("images", existTeam.Image);
            path.DeleteFileFromLocal();

            await _teamRepo.DeleteAsync(existTeam);
        }

        public async Task EditAsync(int? id, TeamEditDto model)
        {
            var existTeam = await _teamRepo.GetById((int)id) ?? throw new NotFoundException("Data not found");

            if (model.UploadImage is not null)
            {
                string oldPath = _env.GenerateFilePath("images", existTeam.Image);
                oldPath.DeleteFileFromLocal();

                string fileName = Guid.NewGuid().ToString() + "-" + model.UploadImage.FileName;

                string newPath = _env.GenerateFilePath("images", fileName);

                await model.UploadImage.SaveFileToLocalAsync(newPath);

                model.Image = fileName;

            }
            _mapper.Map(model, existTeam);



            await _teamRepo.EditAsync(existTeam);
        }

        public async Task<IEnumerable<TeamDto>> GetAllAsync()
        {
            return _mapper.Map<List<TeamDto>>(await _teamRepo.FindAllWithIncludes()
              .Include(m=>m.Socials)
              .ToListAsync());
        }

        public async Task<TeamDto> GetByIdAsync(int? id)
        {
            if (id is null) throw new ArgumentNullException();

            var existTeam = await _teamRepo.GetById((int)id) ?? throw new NotFoundException("Data not found");

            if (existTeam is null) throw new NullReferenceException();

            return _mapper.Map<TeamDto>(existTeam);
        }

        public async Task<PaginationResponse<TeamDto>> GetPaginateDataAsync(int page, int take)
        {
            var team = await _teamRepo.GetAllAsync();
            int totalPage = (int)Math.Ceiling((decimal)team.Count() / take);

            var mappedDatas = _mapper.Map<IEnumerable<TeamDto>>(await _teamRepo.GetPaginateDataAsync(page, take));
            return new PaginationResponse<TeamDto>(mappedDatas, totalPage, page);
        }
    }
}
