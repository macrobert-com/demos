using InheritedIdentityRole.Dto;
using Microsoft.AspNetCore.Identity;

namespace InheritedIdentityRole.Services;

public interface IAuthenticationService
{
    Task<IdentityResult> RegisterUser(UserRegistrationDto userForRegistration);
    Task<bool> ValidateUser(UserAuthenticationDto userForAuth);
    Task<string> CreateToken();
}
