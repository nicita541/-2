using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.Abstractions;
using TaskManager.Application.Auth.Services;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Auth;
using TaskManager.Infrastructure.Common;
using TaskManager.Infrastructure.Persistence;
using TaskManager.Infrastructure.Permissions;

namespace TaskManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(options =>
        {
            var section = configuration.GetSection(JwtOptions.SectionName);
            options.Issuer = section["Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer is missing.");
            options.Audience = section["Audience"] ?? throw new InvalidOperationException("Jwt:Audience is missing.");
            options.Secret = section["Secret"] ?? throw new InvalidOperationException("Jwt:Secret is missing.");
            options.AccessTokenMinutes = int.TryParse(section["AccessTokenMinutes"], out var minutes) ? minutes : 60;
        });
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPasswordHasher<ApplicationUser>, PasswordHasher<ApplicationUser>>();
        services.AddScoped<TaskManager.Application.Abstractions.IPasswordHasher, PasswordHasherAdapter>();
        services.AddScoped<IDateTimeProvider, SystemDateTimeProvider>();
        services.AddScoped<IPermissionService, PermissionService>();

        return services;
    }
}
