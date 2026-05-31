using ErpSystem.Application.Common;
using ErpSystem.Application.Modules.Auth;
using ErpSystem.Application.Modules.Contractors;
using ErpSystem.Application.Modules.Dashboard;
using ErpSystem.Application.Modules.Finance;
using ErpSystem.Application.Modules.HR;
using ErpSystem.Application.Modules.System.Foundation;
using ErpSystem.Application.Modules.Transport;
using ErpSystem.Application.Modules.Warehouse;
using ErpSystem.Infrastructure.Modules.Auth;
using ErpSystem.Infrastructure.Modules.Contractors;
using ErpSystem.Infrastructure.Modules.Dashboard;
using ErpSystem.Infrastructure.Modules.Finance;
using ErpSystem.Infrastructure.Modules.HR;
using ErpSystem.Infrastructure.Modules.System.Foundation;
using ErpSystem.Infrastructure.Modules.Transport;
using ErpSystem.Infrastructure.Modules.Warehouse;
using ErpSystem.Infrastructure.Persistence;
using ErpSystem.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ErpSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString,
        Action<JwtOptions>? configureJwt = null,
        Action<BootstrapAdminOptions>? configureBootstrapAdmin = null)
    {
        services.AddHttpContextAccessor();
        services.AddSingleton<ICurrentUserAccessor, DefaultCurrentUserAccessor>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtTokenFactory, JwtTokenFactory>();
        services.AddDbContext<ErpSystemDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.Configure<JwtOptions>(options => configureJwt?.Invoke(options));
        services.Configure<BootstrapAdminOptions>(options => configureBootstrapAdmin?.Invoke(options));

        services.AddScoped<IAuthRepository, EfAuthRepository>();
        services.AddScoped<ISystemFoundationRepository, EfSystemFoundationRepository>();
        services.AddScoped<IContractorRepository, EfContractorRepository>();
        services.AddScoped<IDashboardRepository, EfDashboardRepository>();
        services.AddScoped<IFinanceRepository, EfFinanceRepository>();
        services.AddScoped<IHrRepository, EfHrRepository>();
        services.AddScoped<ITransportRepository, EfTransportRepository>();
        services.AddScoped<IWarehouseRepository, EfWarehouseRepository>();
        services.AddScoped<AdminAccountInitializer>();

        return services;
    }
}
