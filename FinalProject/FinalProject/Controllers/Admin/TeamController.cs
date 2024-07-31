using Microsoft.AspNetCore.Mvc;
using Service.DTOs.Admin.Teams;
using Service.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace FinalProject.Controllers.Admin
{
    public class TeamController :BaseController
    {
        private readonly ITeamService _teamService;

        public TeamController(ITeamService teamService)
        {
            _teamService = teamService;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] TeamCreateDto request)
        {
            await _teamService.CreateAsync(request);
            return CreatedAtAction(nameof(Create), request);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery][Required] int id)
        {
            await _teamService.DeleteAsync(id);
            return Ok();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromForm] TeamEditDto request)
        {
            await _teamService.EditAsync(id, request);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _teamService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            return Ok(await _teamService.GetByIdAsync(id));
        }
        [HttpGet]
        public async Task<IActionResult> GetPaginateDatas([FromQuery] int page = 1, [FromQuery] int take = 2)
        {
            return Ok(await _teamService.GetPaginateDataAsync(page, take));
        }
    }
}
