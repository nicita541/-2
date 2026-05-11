using TaskManager.Domain.Entities;

namespace TaskManager.Application.Abstractions;

public interface ITokenService
{
    string CreateAccessToken(ApplicationUser user);
}
