using System.Security.Claims;

namespace PaginationResultWebApi.UseCases.Auth.Dtos;

public class SocialAuthUserClaimsDto
{
    public string Email { get; set; } = string.Empty;
    public string GivenName { get; set; } = string.Empty;
    public string SurName { get; set; } = string.Empty;
    public string Picture { get; set; } = string.Empty;

    public static SocialAuthUserClaimsDto FromClaims(IEnumerable<Claim> claims)
    {
        return new SocialAuthUserClaimsDto
        {
            Email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? string.Empty,
            GivenName = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value ?? string.Empty,
            SurName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value ?? string.Empty,
            Picture = claims.FirstOrDefault(c => c.Type == "picture")?.Value ?? string.Empty
        };
    }
}