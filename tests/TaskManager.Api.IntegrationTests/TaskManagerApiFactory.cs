using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TaskManager.Application.Abstractions;

namespace TaskManager.Api.IntegrationTests;

public sealed class TaskManagerApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IApplicationDbContext>();
            services.AddSingleton<IApplicationDbContext, InMemoryApplicationDbContext>();
        });
    }
}
