using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace InheritedIdentityRole.Auth;

public interface IInheritableRoleStore : IRoleStore<InheritableIdentityRole>
{
    Task<List<InheritableIdentityRole>> GetInheritedRolesAsync(InheritableIdentityRole role, CancellationToken cancellationToken = default(CancellationToken));
    Task<IList<Claim>> GetClaimsAsync(InheritableIdentityRole role, CancellationToken cancellationToken = default(CancellationToken));
}