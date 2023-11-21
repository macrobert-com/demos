using AspNetWebApiSqlite.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using Humanizer;
using InheritedIdentityRole.Auth;

namespace AspNetWebApiSqlite.Data;

public class TodosDbContext(DbContextOptions options) : IdentityDbContext<IdentityUser, InheritableIdentityRole, string>(options)
{
    public DbSet<Todo> Todos { get; set; }
    public DbSet<RoleHierarchy> RoleHierarchies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new RoleHierarchyConfiguration());
    }
}
