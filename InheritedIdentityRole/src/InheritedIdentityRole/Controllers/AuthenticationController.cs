using AspNetWebApiSqlite.Data;
using InheritedIdentityRole.Dto;
using InheritedIdentityRole.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace AspNetWebApiSqlite.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class AuthenticationController : ControllerBase
{
    private readonly TodosDbContext context;
    private readonly IAuthenticationService authenticationService;

    public AuthenticationController(TodosDbContext context, IAuthenticationService authenticationService)
    {
        this.context = context;
        this.authenticationService = authenticationService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationDto userForRegistration)
    {
        var result = await authenticationService.RegisterUser(userForRegistration);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.TryAddModelError(error.Code, error.Description);
            }
            return BadRequest(ModelState);
        }
        return StatusCode(201);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Authenticate([FromBody] UserAuthenticationDto user)
    {
        if (!await authenticationService.ValidateUser(user))
        {
            return Unauthorized();
        }

        return Ok(new { Token = await authenticationService.CreateToken() });
    }
}