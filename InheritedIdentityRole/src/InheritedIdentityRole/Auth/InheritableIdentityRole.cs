using Microsoft.AspNetCore.Identity;

namespace InheritedIdentityRole.Auth;

public class InheritableIdentityRole : InheritableIdentityRole<string>
{
    public InheritableIdentityRole()
    {
        Id = Guid.NewGuid().ToString();
    }

    public InheritableIdentityRole(string roleName) : this()
    {
        Name = roleName;
    }
}

public class InheritableIdentityRole<TKey> 
    : IdentityRole<TKey>
    where TKey : IEquatable<TKey>
{
    public ICollection<RoleHierarchy> ChildRoles { get; set; }
}
