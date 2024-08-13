using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Account
{
    public class LoginDto
    {
        public string EmailOrUsername { get; set; }
        public string Password { get; set; }

        public class LoginDtoValidator : AbstractValidator<LoginDto>
        {
            public LoginDtoValidator()
            {
                RuleFor(x => x.EmailOrUsername).NotNull().WithMessage("Username or email is required");
                RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
            }
        }
    }
}
