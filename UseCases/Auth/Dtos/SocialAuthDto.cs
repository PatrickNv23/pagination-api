using System.Security.Claims;

namespace PaginationResultWebApi.UseCases.Auth.Dtos;

public class SocialAuthDto
{
    public string Action { get; set; } = string.Empty;
    public bool Succeeded { get; set; }
    public IEnumerable<Claim> UserClaims { get; set; } = [];
    public string Provider { get; set; } = string.Empty;
}