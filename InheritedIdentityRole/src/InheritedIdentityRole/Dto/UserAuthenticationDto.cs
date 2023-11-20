namespace InheritedIdentityRole.Dto;

public record UserAuthenticationDto
{
    public string? UserName { get; init; }
    public string? Password { get; init; }
}