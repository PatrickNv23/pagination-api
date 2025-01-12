using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaginationResultWebApi.UseCases.Auth.Commands;

namespace PaginationResultWebApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    
    [HttpGet("GoogleAuth")]
    [Authorize(AuthenticationSchemes = GoogleDefaults.AuthenticationScheme)]
    public async Task<IActionResult> GoogleAuth([FromQuery] GoogleAuthCommand googleAuthCommand)
    {
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        
        googleAuthCommand.Succeeded = result.Succeeded;
        googleAuthCommand.UserClaims = result?.Principal?.Claims;

        var apiResponse = await _mediator.Send(googleAuthCommand);
            
        return Redirect(apiResponse?.Data?.ToString()!);
    }
}