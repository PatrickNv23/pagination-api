using MediatR;
using PaginationResultWebApi.Common;
using PaginationResultWebApi.Services.Contracts;

namespace PaginationResultWebApi.UseCases.Auth.Commands;

public class RefreshTokenCommand : IRequest<AuthSessionResponse>
{
    public string ExpiredAccessToken { get; set; }
    public string RefreshToken { get; set; }
}

public class RefreshTokenCommandHandler(IAuthService _authService) : IRequestHandler<RefreshTokenCommand, AuthSessionResponse>
{
    public async Task<AuthSessionResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        return await _authService.RefreshToken(request);
    }
}