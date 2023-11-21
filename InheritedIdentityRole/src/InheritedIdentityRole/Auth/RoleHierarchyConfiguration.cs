using AspNetWebApiSqlite.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace InheritedIdentityRole.Auth;

public class RoleHierarchyConfiguration : IEntityTypeConfiguration<RoleHierarchy>
{
    public void Configure(EntityTypeBuilder<RoleHierarchy> builder)
    {
        builder
            .HasOne(rh => rh.ParentRole)
            .WithMany(r => r.ChildRoles)
            .HasForeignKey(rh => rh.ParentRoleId)
            .IsRequired();

        builder
            .HasOne(rh => rh.ChildRole)
            .WithMany()
            .HasForeignKey(rh => rh.ChildRoleId)
            .IsRequired();

        builder.HasData(
            new RoleHierarchy
            {
                ParentRoleId = RoleConfiguration.UserRoleId,
                ChildRoleId = RoleConfiguration.ManagerRoleId, // Manager 'inherits' User roles
            },
            new RoleHierarchy
            {
                ParentRoleId = RoleConfiguration.ManagerRoleId, 
                ChildRoleId = RoleConfiguration.AdministratorRoleId, // Administrator 'inherits' Manager roles
            });
    }
}