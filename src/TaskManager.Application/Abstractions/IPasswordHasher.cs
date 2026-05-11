using TaskManager.Domain.Entities;

namespace TaskManager.Application.Abstractions;

public interface IPasswordHasher
{
    string HashPassword(ApplicationUser user, string password);
    bool VerifyPassword(ApplicationUser user, string hashedPassword, string password);
}
