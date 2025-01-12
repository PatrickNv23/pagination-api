using PaginationResultWebApi.Entities;
using PaginationResultWebApi.UseCases.Auth.Commands;

namespace PaginationResultWebApi.Services.Contracts;

public interface IAuthService
{
    Task<ApiResponse> GoogleAuth(GoogleAuthCommand googleAuthCommand);
}