using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Auth.Dtos;
using TaskManager.Application.Auth.Services;

namespace TaskManager.Api.Controllers;

public sealed class AuthController(IAuthService authService) : ApiControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.RegisterAsync(request, cancellationToken);
        return ToActionResult(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(request, cancellationToken);
        return ToActionResult(result);
    }
}
