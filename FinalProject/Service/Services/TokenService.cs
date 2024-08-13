using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Service.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Service.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        private readonly IConfiguration _config;
        public TokenService(IConfiguration config)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWTSettings:Key"]));
            _config = config;
        }


        public string CreateToken(AppUser user, IList<string> roles)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId,user.UserName),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim("FullName",user.FullName),
                new Claim(ClaimTypes.NameIdentifier,user.Id)
            };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
            SigningCredentials credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = credentials,
                Audience = _config["Jwt:Audience"],
                Issuer = _config["Jwt:Issuer"]

            };
            JwtSecurityTokenHandler securityTokenHandler = new JwtSecurityTokenHandler();
            var token = securityTokenHandler.CreateToken(tokenDescriptor);
            return securityTokenHandler.WriteToken(token);



        }
    }
}
