using System.Security.Claims;

namespace PaginationResultWebApi.Services.Contracts;

public interface IJwtTokenService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateAccessToken(string token, bool ignoreExpiration);
}