using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Service.DTOs.Account;
using Service.Helpers.Account;
using Service.Helpers.Exceptions;
using Service.Helpers.Enums;
using Service.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.Web;
using Service.Helpers.Extensions;

namespace Service.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly JwtSettings _jwtSettings;
        private readonly IWebHostEnvironment _env;
        private readonly IEmailService _emailService;

        public AccountService(UserManager<AppUser> userManager,
                              RoleManager<IdentityRole> roleManager,
                              IMapper mapper,
                              IOptions<JwtSettings> jwtSettings,
                              IWebHostEnvironment env,
                              IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _jwtSettings = jwtSettings.Value;
            _env = env;
            _emailService = emailService;
        }
        public async Task<RegisterResponse> SignUpAsync(RegisterDto model)
        {
            var user = _mapper.Map<AppUser>(model);

            IdentityResult result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return new RegisterResponse
                {
                    Success = false,
                    Errors = result.Errors.Select(m => m.Description)
                };
            }

            await _userManager.AddToRoleAsync(user, Roles.Member.ToString());

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string url = $"http://localhost:44356/Account/Verify/{HttpUtility.UrlEncode(user.Email)}/{HttpUtility.UrlEncode(token)}";
            string path = _env.GenerateFilePath("templates", "confirm.html");

            string html = await path.ReadFromFileAsync();

            string confirmHtml = html.Replace("verify-link", url);
            _emailService.Send(user.Email, "Email confirmation", confirmHtml);

            return new RegisterResponse
            {
                Success = true,
                Errors = null
            };
        }
        public async Task<bool> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            await _userManager.ConfirmEmailAsync(user, token);
            return true;
        }
        public async Task<LoginResponse> SignInAsync(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.EmailOrUsername) ??
                       await _userManager.FindByNameAsync(model.EmailOrUsername);

            if (user is null)
            {
                return new LoginResponse
                {
                    Success = false,
                    Error = "Login failed",
                    Token = null
                };
            }


            bool result = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!result)
            {
                return new LoginResponse
                {
                    Success = false,
                    Error = "Login failed",
                    Token = null
                };
            }

            List<string> userRoles = (List<string>)await _userManager.GetRolesAsync(user);

            string token = GenerateJwtToken(user.UserName, userRoles);

            return new LoginResponse
            {
                Success = true,
                Error = null,
                Token = token
            };
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            return _mapper.Map<IEnumerable<UserDto>>(await _userManager.Users.ToListAsync());
        }

        public async Task<UserDto> GetUserByUserNameAsync(string userName)
        {
            var existUser = await _userManager.FindByNameAsync(userName);

            return existUser is null
                ? throw new NotFoundException($"{userName} - user not found")
                : _mapper.Map<UserDto>(existUser);
        }

        public async Task CreateRoleAsync()
        {
            foreach (var role in Enum.GetValues(typeof(Roles)))
            {
                if (!await _roleManager.RoleExistsAsync(role.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole { Name = role.ToString() });
                }
            }
        }

        private string GenerateJwtToken(string username, List<string> roles)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, username),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.NameIdentifier, username)
            };

            roles.ForEach(role =>
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            });

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_jwtSettings.ExpireDays));

            var token = new JwtSecurityToken(
                _jwtSettings.Issuer,
                _jwtSettings.Issuer,
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
