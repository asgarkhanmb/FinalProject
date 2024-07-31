using Repository.Helpers;
using Service.DTOs.Admin.Teams;


namespace Service.Services.Interfaces
{
    public interface ITeamService
    {
        Task<IEnumerable<TeamDto>> GetAllAsync();
        Task<TeamDto> GetByIdAsync(int? id);
        Task CreateAsync(TeamCreateDto model);
        Task DeleteAsync(int? id);
        Task EditAsync(int? id, TeamEditDto model);
        Task<PaginationResponse<TeamDto>> GetPaginateDataAsync(int page, int take);
    }
}
