using AspNetWebApiSqlite.Data;
using InheritedIdentityRole.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace InheritedIdentityRole.DependencyInjection;

public static class IdentityExtensions
{
    // We're not trying to build a secure system here. We just want to introduce a basic Auth system
    public static void ConfigureIdentity(this IServiceCollection services)
    {
        var builder = services
            .AddBasicIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<TodosDbContext>()
            .AddDefaultTokenProviders();
    }

    public static void ConfigureInheritableIdentity(this IServiceCollection services)
    {
        var builder = services
            .AddBasicIdentity<IdentityUser, InheritableIdentityRole>()
            .AddInheritableRoleIdentityFrameworkStores<TodosDbContext, IdentityUser>()
            .AddDefaultTokenProviders();
    }

    private static IdentityBuilder AddBasicIdentity<TUser, TRole>(this IServiceCollection services)
        where TUser : class
        where TRole : class
    {
        return services.AddIdentity<IdentityUser, TRole>(o =>
        {
            o.Password.RequireDigit = false;
            o.Password.RequireLowercase = false;
            o.Password.RequireUppercase = false;
            o.Password.RequireNonAlphanumeric = false;
            o.Password.RequiredLength = 8;
        });
    }
}

public static class InheritedRoleExtensions
{
    public static IdentityBuilder AddInheritableRoleIdentityFrameworkStores<TContext, TUser>(this IdentityBuilder builder)
        where TContext : DbContext
        where TUser : IdentityUser<string>
    {
        var services = builder.Services;
        services.AddScoped<IInheritableRoleStore, InheritableRoleStore<TContext>>();
        services.AddScoped<IRoleStore<InheritableIdentityRole>, InheritableRoleStore<TContext>>();
        services.AddScoped<IUserStore<TUser>, UserStore<TUser, InheritableIdentityRole, TContext>>();
        return builder;
    }
}
