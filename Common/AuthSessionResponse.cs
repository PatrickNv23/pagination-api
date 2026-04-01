namespace PaginationResultWebApi.Common;

public class AuthSessionResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}