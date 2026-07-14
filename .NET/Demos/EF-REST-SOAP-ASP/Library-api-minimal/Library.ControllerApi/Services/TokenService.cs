using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Library.ControllerApi.Services;

public class TokenService : ITokenService
{
    private readonly string? _key;

    private static readonly Dictionary<string, string> Roles =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["ada"] = "admin"
        };

    public TokenService(IConfiguration config)
    {
        _key = config["Jwt:Key"];
    }

    public string Issue(string user, string role)
    {
        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)), SecurityAlgorithms.HmacSha256
        );

        //var role = Roles.GetValueOrDefault(user, "customer");

        var token = new JwtSecurityToken("library-fulfillment", "library-fulfillment-clients",
            new[] {new Claim(ClaimTypes.Name, user), new Claim(ClaimTypes.Role, role)},
            expires: DateTime.UtcNow.AddHours(1), signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}