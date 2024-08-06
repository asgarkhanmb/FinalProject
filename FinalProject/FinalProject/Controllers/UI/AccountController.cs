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
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            if (email == null) return BadRequest("Email not found. Make sure you typed correctly");
            var scheme = HttpContext.Request.Scheme;
            var host = HttpContext.Request.Host.Value;
            ResponseObj responseObj = await _accountService.ForgetPassword(email, scheme, host);
            if (responseObj.StatusCode == (int)StatusCodes.Status400BadRequest) return BadRequest(responseObj.ResponseMessage);
            else if (responseObj.StatusCode == (int)StatusCodes.Status404NotFound) return NotFound(responseObj.ResponseMessage);

            return Ok(responseObj.ResponseMessage);
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromQuery] UserResetPasswordDto userResetPasswordDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            ResponseObj responseObj = await _accountService.ResetPassword(userResetPasswordDto);
            if (responseObj.StatusCode == (int)StatusCodes.Status400BadRequest) return BadRequest(responseObj.ResponseMessage);
            else if (responseObj.StatusCode == (int)StatusCodes.Status404NotFound) return NotFound(responseObj.ResponseMessage);

            return Ok(responseObj.ResponseMessage);
        }
        [HttpPost]
        public async Task<IActionResult> VerifyEmail(string verifyEmail, string token)
        {
            if (VerifyEmail == null || token == null) return BadRequest("Something went wrong");
            ResponseObj responseObj = await _accountService.VerifyEmail(verifyEmail, token);
            if (responseObj.StatusCode == (int)StatusCodes.Status400BadRequest) return BadRequest(responseObj.ResponseMessage);
            else if (responseObj.StatusCode == (int)StatusCodes.Status404NotFound) return NotFound(responseObj.ResponseMessage);

            return Ok(responseObj.ResponseMessage);
        }

    }
}
