using MediatR;
using PaginationResultWebApi.Common;
using PaginationResultWebApi.Services.Contracts;
using System.ComponentModel.DataAnnotations;

namespace PaginationResultWebApi.UseCases.Auth.Commands;

public class LoginCommand : IRequest<AuthSessionResponse>
{
    [Required]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}

public class LoginCommandHandler(IAuthService _authService) : IRequestHandler<LoginCommand, AuthSessionResponse>
{
    public async Task<AuthSessionResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return await _authService.Login(request);
    }
}