using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InheritedIdentityRole.Auth;

namespace AspNetWebApiSqlite.Data;

public class RoleConfiguration : IEntityTypeConfiguration<InheritableIdentityRole>
{
    public static readonly string UserRoleId = Guid.NewGuid().ToString();
    public static readonly string ManagerRoleId = Guid.NewGuid().ToString();
    public static readonly string AdministratorRoleId = Guid.NewGuid().ToString();

    public void Configure(EntityTypeBuilder<InheritableIdentityRole> builder)
    {
        builder.HasData(
            new InheritableIdentityRole
            {
                Id = UserRoleId,
                Name = "User",
                NormalizedName = "USER"
            },
            new InheritableIdentityRole
            {
                Id = ManagerRoleId,
                Name = "Manager",
                NormalizedName = "MANAGER"
            },
            new InheritableIdentityRole
            {
                Id = AdministratorRoleId,
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR"
            });
    }
}