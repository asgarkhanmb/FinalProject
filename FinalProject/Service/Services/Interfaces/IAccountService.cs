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
        Task<ResponseObj> VerifyEmail(string VerifyEmail, string token);
        Task<ResponseObj> ForgetPassword(string email, string requestScheme, string requestHost);
        Task<ResponseObj> ResetPassword(UserResetPasswordDto userResetPasswordDto);
        Task<ResponseObj> AddRoleAsync(string username, string roleName);
        Task<ResponseObj> RemoveRoleAsync(string username, string roleName);
    }
}
