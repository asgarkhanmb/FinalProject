using Microsoft.AspNetCore.Identity;

namespace Service.Helpers.Account
{
    /// <summary>ASP.NET Identity xəta mesajlarının Azərbaycan dilində versiyası.</summary>
    public class AzIdentityErrorDescriber : IdentityErrorDescriber
    {
        private static IdentityError E(string code, string description) => new() { Code = code, Description = description };

        public override IdentityError DefaultError() => E(nameof(DefaultError), "Naməlum xəta baş verdi.");
        public override IdentityError DuplicateUserName(string userName) => E(nameof(DuplicateUserName), $"'{userName}' istifadəçi adı artıq mövcuddur.");
        public override IdentityError DuplicateEmail(string email) => E(nameof(DuplicateEmail), $"'{email}' e-poçtu artıq istifadə olunur.");
        public override IdentityError InvalidUserName(string userName) => E(nameof(InvalidUserName), $"'{userName}' istifadəçi adı yalnız hərf və rəqəmlərdən ibarət ola bilər.");
        public override IdentityError InvalidEmail(string email) => E(nameof(InvalidEmail), $"'{email}' düzgün e-poçt ünvanı deyil.");
        public override IdentityError PasswordTooShort(int length) => E(nameof(PasswordTooShort), $"Şifrə ən azı {length} simvol olmalıdır.");
        public override IdentityError PasswordRequiresNonAlphanumeric() => E(nameof(PasswordRequiresNonAlphanumeric), "Şifrədə ən azı bir simvol (məs. !@#) olmalıdır.");
        public override IdentityError PasswordRequiresDigit() => E(nameof(PasswordRequiresDigit), "Şifrədə ən azı bir rəqəm olmalıdır.");
        public override IdentityError PasswordRequiresLower() => E(nameof(PasswordRequiresLower), "Şifrədə ən azı bir kiçik hərf olmalıdır.");
        public override IdentityError PasswordRequiresUpper() => E(nameof(PasswordRequiresUpper), "Şifrədə ən azı bir böyük hərf olmalıdır.");
        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars) => E(nameof(PasswordRequiresUniqueChars), $"Şifrədə ən azı {uniqueChars} fərqli simvol olmalıdır.");
        public override IdentityError PasswordMismatch() => E(nameof(PasswordMismatch), "Şifrə yanlışdır.");
        public override IdentityError InvalidToken() => E(nameof(InvalidToken), "Link etibarsızdır və ya vaxtı keçib.");
        public override IdentityError UserAlreadyInRole(string role) => E(nameof(UserAlreadyInRole), $"İstifadəçi artıq '{role}' roluna malikdir.");
        public override IdentityError UserNotInRole(string role) => E(nameof(UserNotInRole), $"İstifadəçi '{role}' roluna malik deyil.");
    }
}
