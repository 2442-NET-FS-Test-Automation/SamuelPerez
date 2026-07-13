using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Library.ControllerApi.Services;

public class TokenService : ITokenService
{
    private readonly string _key;

    public TokenService(IConfiguration config)
    {
        
    }
}