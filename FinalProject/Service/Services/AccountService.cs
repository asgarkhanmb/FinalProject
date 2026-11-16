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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;


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
        private readonly string _clientUrl;
        private readonly bool _requireConfirmedEmail;
        public AccountService(UserManager<AppUser> userManager,
                              RoleManager<IdentityRole> roleManager,
                              IMapper mapper,
                              IOptions<JwtSettings> jwtSettings,
                              IWebHostEnvironment env,
                              IEmailService emailService,
                              ITokenService tokenService,
                              UrlHelperService urlHelper,
                              ISendEmail sendEmail,
                              IDistributedCache distributedCache,
                              IConfiguration configuration)
        {
            _clientUrl = (configuration["ClientUrl"] ?? "http://127.0.0.1:5500").TrimEnd('/');
            _requireConfirmedEmail = configuration.GetValue("Auth:RequireConfirmedEmail", true);
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
            string url = $"{_clientUrl}/verify.html?email={HttpUtility.UrlEncode(user.Email)}&token={HttpUtility.UrlEncode(token)}";
            string html = $"<p>Hesabınızı təsdiqləmək üçün <a href=\"{url}\">bu linkə</a> keçin.</p>";

            if (_requireConfirmedEmail)
            {
                _emailService.Send(user.Email, "Email confirmation", html);
            }
            else
            {
                // Təsdiq məcburi deyilsə, e-poçt göndərilə bilməsə belə qeydiyyat uğurlu sayılır
                try { _emailService.Send(user.Email, "Email confirmation", html); }
                catch { /* SMTP xətası qeydiyyatı dayandırmasın */ }
            }

            return new RegisterResponse
            {
                Success = true,
                RequiresEmailConfirmation = _requireConfirmedEmail,
                ResponseMessage = new List<string>()
                {
                    _requireConfirmedEmail
                        ? "Account created. Daxil olmazdan əvvəl e-poçtunuzu təsdiqləyin."
                        : "Hesab uğurla yaradıldı."
                }
            };
        }

        public async Task<LoginResponse> SignInAsync(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.EmailOrUsername) ??
               await _userManager.FindByNameAsync(model.EmailOrUsername);

            if (user == null)
            {
                throw new NotFoundException("Bu istifadəçi adı və ya e-poçt ilə hesab tapılmadı.");
            }
            bool isPasswordCorrect = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!isPasswordCorrect)
            {
                throw new ApplicationException("Şifrə yanlışdır.");
            }

            if (_requireConfirmedEmail && !await _userManager.IsEmailConfirmedAsync(user))
            {
                throw new ForbiddenException("Daxil olmazdan əvvəl e-poçtunuzu təsdiqləyin.");
            }

            List<string> userRoles = (List<string>)await _userManager.GetRolesAsync(user);
            string token = GenerateJwtToken(user, userRoles);

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
            if (userName is null) throw new NotFoundException("User not found");

            var existUser = await _userManager.FindByNameAsync(userName);

            if (existUser is null)
                throw new NotFoundException($"{userName} - user not found");

            var roles = await _userManager.GetRolesAsync(existUser);
            var userDto = _mapper.Map<UserDto>(existUser);
            userDto.Roles = roles.ToList();

            return userDto;
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

        private string GenerateJwtToken(AppUser user, List<string> roles)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.UserName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.NameIdentifier, user.UserName),
                // Frontend basket/wishlist sorğuları üçün istifadəçinin Id-sinə ehtiyac duyur
                new("uid", user.Id),
                new("fullName", user.FullName ?? string.Empty)
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
                    ResponseMessage = "İstifadəçi tapılmadı.",
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
                ResponseMessage = "İstifadəçi tapılmadı.",
                StatusCode = (int)StatusCodes.Status400BadRequest
            };
            string token = await _userManager.GeneratePasswordResetTokenAsync(appUser);
            var urlHelper = _urlHelper.GetUrlHelper();
            string link = $"{_clientUrl}/reset-password.html?email={HttpUtility.UrlEncode(appUser.Email)}&token={HttpUtility.UrlEncode(token)}";
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
                ResponseMessage = "Link etibarsızdır və ya vaxtı keçib."
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
                ResponseMessage = "Şifrə uğurla yeniləndi."
            };
        }

        public async Task<ResponseObj> AddRoleAsync(string username, string roleName)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null || user.IsDeleted)
            {
                return new ResponseObj
                {
                    ResponseMessage = "İstifadəçi tapılmadı.",
                    StatusCode = (int)StatusCodes.Status400BadRequest
                };
            }

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                return new ResponseObj
                {
                    ResponseMessage = "Belə rol yoxdur.",
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
                    ResponseMessage = "İstifadəçi tapılmadı.",
                    StatusCode = (int)StatusCodes.Status400BadRequest
                };
            }
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                return new ResponseObj
                {
                    ResponseMessage = "Belə rol yoxdur.",
                    StatusCode = (int)StatusCodes.Status400BadRequest
                };
            }
            if (roleName.Equals(Roles.Admin.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return new ResponseObj
                {
                    ResponseMessage = "'Admin' rolu silinə bilməz.",
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
