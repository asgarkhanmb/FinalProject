using Microsoft.AspNetCore.Mvc;
using Service.DTOs.Account;
using Service.Helpers.Account;


namespace Service.Services.Interfaces
{
    public interface IAccountService
    {
        Task<RegisterResponse> SignUpAsync(RegisterDto model);
        Task<LoginResponse> SignInAsync(LoginDto model);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserByUserNameAsync(string userName);
        Task CreateRoleAsync();
        Task<bool> ConfirmEmailAsync(string userId, string token);
    }
}
