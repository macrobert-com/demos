using AutoMapper;
using InheritedIdentityRole.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InheritedIdentityRole.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IMapper mapper;
    private readonly UserManager<IdentityUser> userManager;
    private readonly IConfiguration configuration;
    private IdentityUser? _user;

    public AuthenticationService(IMapper mapper, UserManager<IdentityUser> userManager, IConfiguration configuration)
    {
        this.mapper = mapper;
        this.userManager = userManager;
        this.configuration = configuration;
    }

    public async Task<IdentityResult> RegisterUser(UserRegistrationDto userForRegistration)
    {
        var user = mapper.Map<IdentityUser>(userForRegistration);
        var result = await userManager.CreateAsync(user, userForRegistration.Password);
        if (result.Succeeded)
        {
            await userManager.AddToRolesAsync(user, userForRegistration.Roles);
        }
        return result;
    }

    public async Task<bool> ValidateUser(UserAuthenticationDto userForAuth)
    {
        _user = await userManager.FindByNameAsync(userForAuth.UserName); // For brevity and demo we'll permit coding with side-effects
        return (_user != null && await userManager.CheckPasswordAsync(_user, userForAuth.Password));
    }

    public async Task<string> CreateToken()
    {
        var signingCredentials = GetSigningCredentials();
        var claims = await GetClaims();
        var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }

    private SigningCredentials GetSigningCredentials()
    {
        // Don't do this...For demonstration only:
        var key = Encoding.UTF8.GetBytes("MySecretKeyIsSuperLongAndSuperStrong");
        // Do this instead:
        // var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET"));
        var secret = new SymmetricSecurityKey(key);
        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var tokenOptions = new JwtSecurityToken
        (
            issuer: jwtSettings["validIssuer"],
            audience: jwtSettings["validAudience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["expires"])),
            signingCredentials: signingCredentials
        );
        return tokenOptions;
    }

    private async Task<List<Claim>> GetClaims()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, _user.UserName)
        };
        var roles = await userManager.GetRolesAsync(_user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        return claims;
    }
}