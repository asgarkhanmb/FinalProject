using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs.Account;
using Service.Helpers.Account;
using Service.Services.Interfaces;

namespace FinalProject.Controllers.Admin
{
    [Authorize("Admin")]
    public class AccountController : BaseController
    {

        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(await _accountService.GetAllUsersAsync());
        }
        [HttpGet]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            return Ok(await _accountService.GetUserByUserNameAsync(username));
        }
        [HttpPost]
        public async Task<IActionResult> CreateRoles()
        {
            await _accountService.CreateRoleAsync();
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> AddRole([FromQuery] RoleAssignmentDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.RoleName))
            {
                return BadRequest("Invalid request.");
            }

            var response = await _accountService.AddRoleAsync(request.Username, request.RoleName);

            if (response.StatusCode == StatusCodes.Status200OK)
            {
                return Ok(response.ResponseMessage);
            }
            else
            {
                return BadRequest(response.ResponseMessage);
            }
        }
        [HttpPost]
        public async Task<IActionResult> RemoveRole([FromQuery] RoleAssignmentDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.RoleName))
            {
                return BadRequest("Invalid request.");
            }

            var response = await _accountService.RemoveRoleAsync(request.Username, request.RoleName);

            if (response.StatusCode == StatusCodes.Status200OK)
            {
                return Ok(response.ResponseMessage);
            }
            else
            {
                return BadRequest(response.ResponseMessage);
            }
        }
    }
}
