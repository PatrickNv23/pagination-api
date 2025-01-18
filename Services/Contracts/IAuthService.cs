using PaginationResultWebApi.Common;
using PaginationResultWebApi.UseCases.Auth.Commands;

namespace PaginationResultWebApi.Services.Contracts;

public interface IAuthService
{
    Task<ApiResponse> GoogleAuth(GoogleAuthCommand googleAuthCommand);
    Task<AuthSessionResponse> Login(LoginCommand loginCommand);
    Task<AuthSessionResponse> RefreshToken(RefreshTokenCommand refreshToken);
}