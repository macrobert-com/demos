using LinqToDB;
using LinqToDB.EntityFrameworkCore;
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

    static InheritableRoleStore()
    {
        LinqToDBForEFTools.Initialize();
    }

    public async Task<List<InheritableIdentityRole>> GetInheritedRolesAsync(InheritableIdentityRole role, CancellationToken cancellationToken = default(CancellationToken))
    {
        using var dc = Context.CreateLinqToDBConnection();

        var recursiveRoleCte = dc.GetCte<InheritableIdentityRole>("InheritedRoles", query =>        
           (from r in Roles.Where(r => r.Id == role.Id) select r)
            .Concat(
                from rh in RoleHierarchies
                join cq in query on rh.ChildRoleId equals cq.Id
                select new InheritableIdentityRole { Id = rh.ParentRoleId, Name = cq.Name })
        );

        var cteQuery =
            from cteRow in recursiveRoleCte
            join r in Roles on cteRow.Id equals r.Id
            select r;

        var inheritedRoles = await cteQuery.ToListAsyncLinqToDB(cancellationToken);
        return inheritedRoles;
    }
}
