using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Dapper;

namespace InheritedIdentityRole.Auth;

public class InheritableRoleStore<TContext>
    : Microsoft.AspNetCore.Identity.EntityFrameworkCore.RoleStore<InheritableIdentityRole, TContext>
    , IInheritableRoleStore
    where TContext : DbContext
{
    public InheritableRoleStore(TContext context, IdentityErrorDescriber? describer = null) : base(context, describer)
    {}

    private DbSet<RoleHierarchy> RoleHierarchies { get { return Context.Set<RoleHierarchy>(); } }
    private DbSet<IdentityRoleClaim<string>> RoleClaims { get { return Context.Set<IdentityRoleClaim<string>>(); } }

    public async Task<List<InheritableIdentityRole>> GetInheritedRolesAsync(InheritableIdentityRole role, CancellationToken cancellationToken = default(CancellationToken))
    {
        const string InheritedRolesQuery = @"
WITH InheritedRoles AS (
    SELECT r.Id, r.Name
    FROM AspNetRoles r
    WHERE r.Id = @RoleId
    UNION ALL
    SELECT r.Id, r.Name
    FROM AspNetRoles r
    JOIN RoleHierarchies ir ON r.Id = ir.ParentRoleId
    JOIN InheritedRoles ON ir.ChildRoleId = InheritedRoles.Id
)
SELECT InheritedRoles.Id, InheritedRoles.Name FROM InheritedRoles;
";
        using var connection = Context.Database.GetDbConnection();
        var result = await connection.QueryAsync<InheritableIdentityRole>(InheritedRolesQuery, new { RoleId = role.Id });
        var inheritedRoles = new List<InheritableIdentityRole>(result);
        return inheritedRoles;
    }

    private async Task<List<InheritableIdentityRole>> GetChildRolesAsync(InheritableIdentityRole role, CancellationToken cancellationToken)
    {
        return await RoleHierarchies
            .Where(rh => rh.ChildRoleId == role.Id)
            .Select(rh => rh.ParentRole)
            .ToListAsync(cancellationToken);
    }

    public override async Task<IList<Claim>> GetClaimsAsync(InheritableIdentityRole role, CancellationToken cancellationToken = default(CancellationToken))
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(role);

        var allRoles = await GetInheritedRolesAsync(role);
        allRoles.Add(role);

        var allClaims = new List<Claim>();
        allRoles.ForEach(async r =>
        {
            var roleClaims = await RoleClaims
                .Where(rc => rc.RoleId.Equals(r.Id))
                .Select(c => new Claim(c.ClaimType!, c.ClaimValue!))
                .ToListAsync(cancellationToken);

            allClaims.AddRange(roleClaims);
        });

        return allClaims;
    }
}
