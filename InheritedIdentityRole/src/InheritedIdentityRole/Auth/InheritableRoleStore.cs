using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
        var inheritedRoles = new List<InheritableIdentityRole>();

        var rolesToProcess = new Queue<InheritableIdentityRole>();
        rolesToProcess.Enqueue(role);

        while (rolesToProcess.Count > 0)
        {
            var currentRole = rolesToProcess.Dequeue();
            var parentRoles = await GetChildRolesAsync(currentRole, cancellationToken);

            foreach (var parentRole in parentRoles)
            {
                // Avoid adding duplicate roles
                if (!inheritedRoles.Contains(parentRole))
                {
                    inheritedRoles.Add(parentRole);
                    rolesToProcess.Enqueue(parentRole);
                }
            }
        }

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
