﻿using Microsoft.AspNetCore.Mvc;
using Service.DTOs.Account;
using Service.Helpers.Account;
using Service.Services.Interfaces;

namespace FinalProject.Controllers.Admin
{
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

        [HttpPost("VerifyEmail")]
        public async Task<IActionResult> VerifyEmail(string verifyEmail, string token)
        {
            if (VerifyEmail == null || token == null) return BadRequest("Something went wrong");
            ResponseObj responseObj = await _accountService.VerifyEmail(verifyEmail, token);
            if (responseObj.StatusCode == (int)StatusCodes.Status400BadRequest) return BadRequest(responseObj.ResponseMessage);
            else if (responseObj.StatusCode == (int)StatusCodes.Status404NotFound) return NotFound(responseObj.ResponseMessage);

            return Ok(responseObj.ResponseMessage);
        }
        [HttpPost("ForgetPassword")]
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
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] UserResetPasswordDto userResetPasswordDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            ResponseObj responseObj = await _accountService.ResetPassword(userResetPasswordDto);
            if (responseObj.StatusCode == (int)StatusCodes.Status400BadRequest) return BadRequest(responseObj.ResponseMessage);
            else if (responseObj.StatusCode == (int)StatusCodes.Status404NotFound) return NotFound(responseObj.ResponseMessage);

            return Ok(responseObj.ResponseMessage);
        }
    }
}
