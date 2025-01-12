using System.Security.Claims;

namespace PaginationResultWebApi.UseCases.Auth.Dtos;

public class SocialAuthUserClaimsDto
{
    public string Email { get; set; }
    public string GivenName { get; set; }
    public string SurName { get; set; }
    public string Picture { get; set; }

    public static SocialAuthUserClaimsDto FromClaims(IEnumerable<Claim> claims)
    {
        return new SocialAuthUserClaimsDto
        {
            Email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
            GivenName = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value,
            SurName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value,
            Picture = claims.FirstOrDefault(c => c.Type == "picture")?.Value
        };
    }
}