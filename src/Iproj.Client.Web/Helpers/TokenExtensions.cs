using Iproj.Client.Web.Client.Models;
using IdentityModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Iproj.Client.Web.Client.Helpers;

public static class TokenExtensions
{
    public static ClaimsPrincipal DecodeJwtToken(this string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var identity = new ClaimsIdentity(jwtToken.Claims, "jwt");
        return new ClaimsPrincipal(identity);
    }

    public static string GetClaimValue(this ClaimsPrincipal claimsPrincipal, string claimType)
    {
        return claimsPrincipal?.Claims.FirstOrDefault(c => c.Type == claimType)?.Value!;
    }

    public static string GetRole(this ClaimsPrincipal user)
    {
        var roleClaim = user.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Role);
        return roleClaim?.Value!;
    }
}
