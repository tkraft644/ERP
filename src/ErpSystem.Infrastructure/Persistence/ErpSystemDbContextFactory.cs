using ErpSystem.Application.Common;
using ErpSystem.Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ErpSystem.Infrastructure.Persistence;

public sealed class ErpSystemDbContextFactory : IDesignTimeDbContextFactory<ErpSystemDbContext>
{
    public ErpSystemDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ErpSystemDbContext>();
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__ErpDb")
            ?? Environment.GetEnvironmentVariable("ConnectionStrings__ErpSystemDatabase")
            ?? Environment.GetEnvironmentVariable("ERPSYSTEM_CONNECTION_STRING")
            ?? "Server=localhost,11433;Database=ErpSystemDb;User Id=sa;Password=ErpSqlLocal2026!;TrustServerCertificate=True;MultipleActiveResultSets=True";
        optionsBuilder.UseSqlServer(connectionString);

        ICurrentUserAccessor currentUserAccessor = new DefaultCurrentUserAccessor(new HttpContextAccessor());
        return new ErpSystemDbContext(optionsBuilder.Options, currentUserAccessor);
    }
}
