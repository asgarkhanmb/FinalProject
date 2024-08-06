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
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Service.Helpers;


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
        private readonly ITokenService _tokenService;
        private readonly UrlHelperService _urlHelper;
        private readonly ISendEmail _sendEmail;
        private readonly IDistributedCache _distributedCache;
        public AccountService(UserManager<AppUser> userManager,
                              RoleManager<IdentityRole> roleManager,
                              IMapper mapper,
                              IOptions<JwtSettings> jwtSettings,
                              IWebHostEnvironment env,
                              IEmailService emailService,
                              ITokenService tokenService,
                              UrlHelperService urlHelper,
                              ISendEmail sendEmail,
                              IDistributedCache distributedCache)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _jwtSettings = jwtSettings.Value;
            _env = env;
            _emailService = emailService;
            _tokenService = tokenService;
            _urlHelper = urlHelper;
            _sendEmail = sendEmail;
            _distributedCache = distributedCache;
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
                    ResponseMessage = result.Errors.Select(m => m.Description)
                };
            }

            await _userManager.AddToRoleAsync(user, Roles.Member.ToString());

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string url = $"https://localhost:44317/Account/Verify/{HttpUtility.UrlEncode(user.Email)}/{HttpUtility.UrlEncode(token)}";
            _emailService.Send(user.Email, "Email confirmation", url);
            return new RegisterResponse
            {
                Success = true,
                ResponseMessage = new List<string>() { token }
            };
        }

        public async Task<LoginResponse> SignInAsync(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.EmailOrUsername) ??
               await _userManager.FindByNameAsync(model.EmailOrUsername);

            if (user == null)
            {
                return new LoginResponse
                {
                    Success = false,
                    Error = "Login failed: User not found",
                    Token = null
                };
            }
            bool isPasswordCorrect = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!isPasswordCorrect)
            {
                return new LoginResponse
                {
                    Success = false,
                    Error = "Login failed: Password or Username incorrect!!",
                    Token = null
                };
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return new LoginResponse
                {
                    Success = false,
                    Error = "Login failed: Email not confirmed",
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
            var users = await _userManager.Users.ToListAsync();
            var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);

            foreach (var userDto in userDtos)
            {
                var roles = await _userManager.GetRolesAsync(await _userManager.FindByNameAsync(userDto.Username));
                userDto.Roles = roles.ToList();
            }

            return userDtos;
        }

        public async Task<UserDto> GetUserByUserNameAsync(string userName)
        {
            if (userName is null) throw new NotFoundException($"User not found");
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

        public async Task<ResponseObj> VerifyEmail(string VerifyEmail, string token)
        {
            AppUser appUser = await _userManager.FindByEmailAsync(VerifyEmail);
            if (appUser == null || appUser.IsDeleted)
            {
                return new ResponseObj
                {
                    ResponseMessage = "User does not exist.",
                    StatusCode = (int)StatusCodes.Status400BadRequest
                };
            }
            IdentityResult resoult = await _userManager.ConfirmEmailAsync(appUser, token);
            if (!resoult.Succeeded)
            {
                return new ResponseObj
                {
                    ResponseMessage = string.Join(", ", resoult.Errors.Select(error => error.Description)),
                    StatusCode = (int)StatusCodes.Status400BadRequest
                };
            }
            await _userManager.UpdateSecurityStampAsync(appUser);
            IList<string> roles = await _userManager.GetRolesAsync(appUser);
            return new ResponseObj
            {
                ResponseMessage = _tokenService.CreateToken(appUser, roles),
                StatusCode = (int)StatusCodes.Status200OK
            };
        }

        public async Task<ResponseObj> ForgetPassword(string email, string requestScheme, string requestHost)
        {
            AppUser appUser = await _userManager.FindByEmailAsync(email);
            if (appUser == null || appUser.IsDeleted) return new ResponseObj
            {
                ResponseMessage = "User does not exist.",
                StatusCode = (int)StatusCodes.Status400BadRequest
            };
            string token = await _userManager.GeneratePasswordResetTokenAsync(appUser);
            var urlHelper = _urlHelper.GetUrlHelper();
            string link = $"https://localhost:44317/reset/{HttpUtility.UrlEncode(appUser.Email)}/{HttpUtility.UrlEncode(token)}";
            _sendEmail.Send("asgarkhanmb@code.edu.az", "Cake Store", appUser.Email, link, "Reset Password");
            IList<string> roles = await _userManager.GetRolesAsync(appUser);
            return new ResponseObj
            {
                ResponseMessage = token,
                StatusCode = (int)StatusCodes.Status200OK
            };
        }

        public async Task<ResponseObj> ResetPassword(UserResetPasswordDto userResetPasswordDto)
        {
            AppUser appUser = await _userManager.FindByEmailAsync(userResetPasswordDto.Email);
            if (appUser == null || appUser.IsDeleted) return new ResponseObj
            {
                ResponseMessage = "User not found",
                StatusCode = (int)StatusCodes.Status404NotFound
            };
            var isSucceeded = await _userManager.VerifyUserTokenAsync(appUser, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", userResetPasswordDto.Token);
            if (!isSucceeded) return new ResponseObj
            {
                StatusCode = (int)StatusCodes.Status400BadRequest,
                ResponseMessage = "TokenIsNotValid"
            };
            IdentityResult resoult = await _userManager.ResetPasswordAsync(appUser, userResetPasswordDto.Token, userResetPasswordDto.Password);
            if (!resoult.Succeeded) return new ResponseObj
            {
                ResponseMessage = string.Join(", ", resoult.Errors.Select(error => error.Description)),
                StatusCode = (int)StatusCodes.Status400BadRequest
            };
            await _userManager.UpdateSecurityStampAsync(appUser);
            await _distributedCache.RemoveAsync(appUser.Email);
            return new ResponseObj
            {
                StatusCode = (int)StatusCodes.Status200OK,
                ResponseMessage = "Password successfully reseted"
            };
        }

        public async Task<ResponseObj> AddRoleAsync(string username, string roleName)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null || user.IsDeleted)
            {
                return new ResponseObj
                {
                    ResponseMessage = "User does not exist.",
                    StatusCode = (int)StatusCodes.Status400BadRequest
                };
            }

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                return new ResponseObj
                {
                    ResponseMessage = "Role does not exist.",
                    StatusCode = (int)StatusCodes.Status400BadRequest
                };
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                return new ResponseObj
                {
                    ResponseMessage = string.Join(", ", result.Errors.Select(error => error.Description)),
                    StatusCode = (int)StatusCodes.Status400BadRequest
                };
            }

            return new ResponseObj
            {
                ResponseMessage = $"Role '{roleName}' added to user '{username}' successfully.",
                StatusCode = (int)StatusCodes.Status200OK
            };
        }
        public async Task<ResponseObj> RemoveRoleAsync(string username, string roleName)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null || user.IsDeleted)
            {
                return new ResponseObj
                {
                    ResponseMessage = "User does not exist.",
                    StatusCode = (int)StatusCodes.Status400BadRequest
                };
            }
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                return new ResponseObj
                {
                    ResponseMessage = "Role does not exist.",
                    StatusCode = (int)StatusCodes.Status400BadRequest
                };
            }
            if (roleName.Equals(Roles.Member.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return new ResponseObj
                {
                    ResponseMessage = "The 'Member' role cannot be removed.",
                    StatusCode = (int)StatusCodes.Status400BadRequest
                };
            }
            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                return new ResponseObj
                {
                    ResponseMessage = string.Join(", ", result.Errors.Select(error => error.Description)),
                    StatusCode = (int)StatusCodes.Status400BadRequest
                };
            }

            return new ResponseObj
            {
                ResponseMessage = $"Role '{roleName}' removed from user '{username}' successfully.",
                StatusCode = (int)StatusCodes.Status200OK
            };
        }

    }
}
