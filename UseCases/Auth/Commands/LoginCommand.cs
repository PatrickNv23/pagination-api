using System.ComponentModel.DataAnnotations;
using MediatR;
using PaginationResultWebApi.Common;
using PaginationResultWebApi.Services.Contracts;

namespace PaginationResultWebApi.UseCases.Auth.Commands;

public class LoginCommand : IRequest<AuthSessionResponse>
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}

public class LoginCommandHandler(IAuthService _authService) : IRequestHandler<LoginCommand, AuthSessionResponse>
{
    public async Task<AuthSessionResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return await _authService.Login(request);
    }
}