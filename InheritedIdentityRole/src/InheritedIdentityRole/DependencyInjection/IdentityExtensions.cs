using AspNetWebApiSqlite.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace InheritedIdentityRole.DependencyInjection;

public static class IdentityExtensions
{
    // We're not trying to build a secure system here. We just want to introduce a basic Auth system
    public static void ConfigureIdentity(this IServiceCollection services)
    {
        var builder = services.AddIdentity<IdentityUser, IdentityRole>(o =>
        {
            o.Password.RequireDigit = false;
            o.Password.RequireLowercase = false;
            o.Password.RequireUppercase = false;
            o.Password.RequireNonAlphanumeric = false;
            o.Password.RequiredLength = 8;
        })
        .AddEntityFrameworkStores<TodosDbContext>()
        .AddDefaultTokenProviders();
    }

    public static void ConfigureJWT(this IServiceCollection services, IConfiguration
    configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        // Don't do this...For demonstration only:
        var secretKeyBytes = Encoding.UTF8.GetBytes("MySecretKeyIsSuperLongAndSuperStrong");
        // Do this instead:
        // var secretKeyBytes = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET"));
        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["validIssuer"],
                ValidAudience = jwtSettings["validAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes)
            };
        });
    }
}
