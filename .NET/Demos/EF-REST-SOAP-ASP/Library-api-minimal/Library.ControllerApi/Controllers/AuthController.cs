using Library.ControllerApi.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokens;

    public AuthController (ITokenService tokens)
    {
        _tokens = tokens;
    }

    [HttpPost("token")]
    public ActionResult IssueToken(string userName)
    {
        var userToken = _tokens.Issue(userName);
        return Ok(userToken);
    }
}