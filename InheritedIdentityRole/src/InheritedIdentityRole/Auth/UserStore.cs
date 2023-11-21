using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace InheritedIdentityRole.Auth;

public class UserStore<TUser, TRole, TContext>
    : Microsoft.AspNetCore.Identity.EntityFrameworkCore.UserStore<TUser, TRole, TContext>
    where TUser : IdentityUser<string>
    where TRole : InheritableIdentityRole
    where TContext : DbContext
{
    public UserStore(IInheritableRoleStore roleStore,  TContext context, IdentityErrorDescriber? describer = null) : base(context, describer)
    {
        RoleStore = roleStore;
    }

    private IInheritableRoleStore RoleStore { get; }
    private DbSet<InheritableIdentityRole> Roles { get { return Context.Set<InheritableIdentityRole>(); } }
    private DbSet<IdentityUserRole<string>> UserRoles { get { return Context.Set<IdentityUserRole<string>>(); } }

    public override async Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(user);

        var userId = user.Id;
        var query = from userRole in UserRoles
                    join role in Roles on userRole.RoleId equals role.Id
                    where userRole.UserId.Equals(userId)
                    select role;
        var roles = await query.ToListAsync(cancellationToken);

        var resultRoles = new HashSet<string>(roles.Select(x => x.Name!));
        roles.ForEach(async role =>
        {
            var inheritedRoles = await RoleStore.GetInheritedRolesAsync(role);
            resultRoles.AddRange(inheritedRoles.Select(x => x.Name));
        });

        return resultRoles.ToList();
    }
}
