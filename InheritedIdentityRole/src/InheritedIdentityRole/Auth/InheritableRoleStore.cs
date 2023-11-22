using LinqToDB;
using LinqToDB.Data;
using LinqToDB.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace InheritedIdentityRole.Auth;

public class InheritableRoleStore<TContext>
    : Microsoft.AspNetCore.Identity.EntityFrameworkCore.RoleStore<InheritableIdentityRole, TContext>
    , IInheritableRoleStore
    where TContext : DbContext
{
    public InheritableRoleStore(ILogger<InheritableRoleStore<TContext>> logger, TContext context, IdentityErrorDescriber? describer = null) 
        : base(context, describer)
    {
        this.Logger = logger;
        DataConnection.WriteTraceLine = (message, category, level) => Logger.LogInformation(message);
    }

    private DbSet<RoleHierarchy> RoleHierarchies { get { return Context.Set<RoleHierarchy>(); } }
    private DbSet<IdentityRoleClaim<string>> RoleClaims { get { return Context.Set<IdentityRoleClaim<string>>(); } }

    public ILogger<InheritableRoleStore<TContext>> Logger { get; }

    static InheritableRoleStore()
    {
        LinqToDBForEFTools.Initialize();
#if DEBUG
        DataConnection.TurnTraceSwitchOn();
#endif
    }

    public async Task<List<InheritableIdentityRole>> GetInheritedRolesAsync(InheritableIdentityRole role, CancellationToken cancellationToken = default(CancellationToken))
    {
        using var dc = Context.CreateLinqToDBConnection();
        //Logger.LogInformation("LinqToDBConnection Created");

        var recursiveRoleCte = dc.GetCte<InheritableIdentityRole>("InheritedRoles", query =>        
           (from r in Roles.Where(r => r.Id == role.Id) select r)
            .Concat(
                from rh in RoleHierarchies
                join cq in query on rh.ChildRoleId equals cq.Id
                select new InheritableIdentityRole { Id = rh.ParentRoleId, Name = cq.Name })
        );
        //Logger.LogInformation("Recursive CTE defined");

        var cteQuery =
            from cteRow in recursiveRoleCte
            join r in Roles on cteRow.Id equals r.Id
            select r;

        //Logger.LogInformation("Final Role Query defined");

        var inheritedRoles = await cteQuery.ToListAsyncLinqToDB(cancellationToken);
        //Logger.LogInformation("Query executed and results materialized");

        return inheritedRoles;
    }
}
