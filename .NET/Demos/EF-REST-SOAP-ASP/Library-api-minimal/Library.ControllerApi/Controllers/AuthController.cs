using Library.ControllerApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Library.ControllerApi.DTOs;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokens;
    private readonly IUserService _users;

    public AuthController (ITokenService tokens, IUserService users)
    {
        _tokens = tokens;
        _users = users;
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterDto dto)
    {
        var error = await _users.RegisterAsync(dto.Username, dto.Password);
        if ( error is not null)
        {
            return Conflict(new { error });
        }

        return CreatedAtAction(nameof(Me), null);
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginDto dto)
    {
        var user = await _users.ValidateAsync(dto.Username, dto.Password);

        if (user is null)
        {
            return Unauthorized(new { error = "bad credentials" });
        }

        return Ok(new {token = _tokens.Issue(user.UserName, user.Role) });
    }

    [HttpGet("me")]
    public ActionResult Me()
    {
        return Ok( new
        {
            name = User.Identity?.Name,
            role = User.FindFirstValue(ClaimTypes.Role)
        });
    }

    [HttpPost("token")]
    public ActionResult IssueToken(string userName)
    {
        var userToken = _tokens.Issue(userName, "consumer");
        return Ok(userToken);
    }
}