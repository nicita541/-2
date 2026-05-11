using Microsoft.AspNetCore.Identity;
using TaskManager.Application.Abstractions;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Auth;

public sealed class PasswordHasherAdapter(IPasswordHasher<ApplicationUser> inner) : TaskManager.Application.Abstractions.IPasswordHasher
{
    public string HashPassword(ApplicationUser user, string password) => inner.HashPassword(user, password);

    public bool VerifyPassword(ApplicationUser user, string hashedPassword, string password) =>
        inner.VerifyHashedPassword(user, hashedPassword, password) != PasswordVerificationResult.Failed;
}
