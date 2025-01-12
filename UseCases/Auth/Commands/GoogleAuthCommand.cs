using System.Security.Claims;
using MediatR;
using PaginationResultWebApi.Entities;
using PaginationResultWebApi.Services.Contracts;

namespace PaginationResultWebApi.UseCases.Auth.Commands;

public class GoogleAuthCommand : IRequest<ApiResponse>
{
    public string Action { get; set; }
    public bool Succeeded { get; set; } = false;
    public IEnumerable<Claim>? UserClaims { get; set; } = null;
}

public class GoogleAuthCommandHandler(IAuthService authService) : IRequestHandler<GoogleAuthCommand, ApiResponse>
{
    public async Task<ApiResponse> Handle(GoogleAuthCommand request, CancellationToken cancellationToken)
    {
        return await authService.GoogleAuth(request);
    }
}