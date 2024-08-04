using Microsoft.AspNetCore.Mvc;
using Service.DTOs.Account;
using Service.Helpers.Account;
using Service.Services.Interfaces;

namespace FinalProject.Controllers.UI
{
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)

        {
            _accountService = accountService;

        }

        [HttpPost]
        public async Task<IActionResult> SignUp([FromQuery] RegisterDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var response = await _accountService.SignUpAsync(request);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> SignIn([FromQuery] LoginDto request)
        {
            return Ok(await _accountService.SignInAsync(request));
        }
    }
}
