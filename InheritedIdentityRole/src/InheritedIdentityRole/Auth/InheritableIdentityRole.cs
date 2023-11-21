using Microsoft.AspNetCore.Identity;

namespace InheritedIdentityRole.Auth;

public class InheritableIdentityRole : IdentityRole<string>
{
    public InheritableIdentityRole()
    {
        Id = Guid.NewGuid().ToString();
    }

    public InheritableIdentityRole(string roleName) : this()
    {
        Name = roleName;
    }

    public ICollection<RoleHierarchy> ChildRoles { get; set; }
}

