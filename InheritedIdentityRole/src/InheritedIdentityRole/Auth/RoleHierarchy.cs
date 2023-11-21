namespace InheritedIdentityRole.Auth;

public class RoleHierarchy
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string ParentRoleId { get; set; }
    public InheritableIdentityRole ParentRole { get; set; }

    public string ChildRoleId { get; set; }
    public InheritableIdentityRole ChildRole { get; set; }
}
