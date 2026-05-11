using TaskManager.Application.Abstractions;
using TaskManager.Application.Auth.Dtos;
using TaskManager.Application.Auth.Services;
using TaskManager.Application.Common;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Auth;

public sealed class AuthService(IApplicationDbContext db, IPasswordHasher passwordHasher, ITokenService tokenService) : IAuthService
{
    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var exists = await db.AnyAsync(db.Users.Where(x => x.Email == email), cancellationToken);
        if (exists) return Result<AuthResponse>.Failure(Error.Conflict("Email is already registered."));

        var user = new ApplicationUser { Email = email, DisplayName = request.DisplayName, PasswordHash = string.Empty };
        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
        db.Add(user);
        await db.SaveChangesAsync(cancellationToken);
        return Result<AuthResponse>.Success(CreateResponse(user));
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await db.FirstOrDefaultAsync(db.Users.Where(x => x.Email == email), cancellationToken);
        if (user is null) return Result<AuthResponse>.Failure(Error.Unauthorized("Invalid email or password."));

        if (!passwordHasher.VerifyPassword(user, user.PasswordHash, request.Password))
        {
            return Result<AuthResponse>.Failure(Error.Unauthorized("Invalid email or password."));
        }

        return Result<AuthResponse>.Success(CreateResponse(user));
    }

    private AuthResponse CreateResponse(ApplicationUser user) =>
        new(user.Id, user.Email, user.DisplayName, tokenService.CreateAccessToken(user));
}
