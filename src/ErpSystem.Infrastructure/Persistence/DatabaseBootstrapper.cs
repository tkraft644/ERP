using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ErpSystem.Infrastructure.Security;

namespace ErpSystem.Infrastructure.Persistence;

public static class DatabaseBootstrapper
{
    public static async Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ErpSystemDbContext>();
        await dbContext.Database.MigrateAsync(cancellationToken);
        var adminAccountInitializer = scope.ServiceProvider.GetRequiredService<AdminAccountInitializer>();
        await adminAccountInitializer.EnsureAdminAsync(cancellationToken);
    }
}
