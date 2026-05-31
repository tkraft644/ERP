using ErpSystem.Domain.Modules.System.Auditing;
using ErpSystem.Domain.Modules.System.Identity;
using ErpSystem.Application.Modules.Auth;
using ErpSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ErpSystem.Infrastructure.Security;

public sealed class AdminAccountInitializer
{
    private readonly ErpSystemDbContext dbContext;
    private readonly IPasswordHasher passwordHasher;
    private readonly BootstrapAdminOptions options;

    public AdminAccountInitializer(
        ErpSystemDbContext dbContext,
        IPasswordHasher passwordHasher,
        IOptions<BootstrapAdminOptions> options)
    {
        this.dbContext = dbContext;
        this.passwordHasher = passwordHasher;
        this.options = options.Value;
    }

    public async Task EnsureAdminAsync(CancellationToken cancellationToken = default)
    {
        var adminRole = await dbContext.Roles.FirstOrDefaultAsync(item => item.Code == "SYSTEM_ADMIN", cancellationToken);
        if (adminRole is null)
        {
            throw new InvalidOperationException("Brakuje roli SYSTEM_ADMIN. Sprawdź migracje i seed uprawnień.");
        }

        var adminUser = await dbContext.Users.FirstOrDefaultAsync(item => item.UserName == options.Login, cancellationToken);

        if (adminUser is null)
        {
            var (hash, salt) = passwordHasher.HashPassword(options.Password);
            adminUser = new User(options.Login, options.Email, options.DisplayName, true);
            adminUser.SetPassword(hash, salt, options.MustChangePassword);

            await dbContext.Users.AddAsync(adminUser, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            var hasRole = await dbContext.UserRoles.AnyAsync(
                item => item.UserId == adminUser.Id && item.RoleId == adminRole.Id,
                cancellationToken);

            if (!hasRole)
            {
                await dbContext.UserRoles.AddAsync(new UserRole(adminUser.Id, adminRole.Id), cancellationToken);
            }

            await dbContext.AuditLogs.AddAsync(
                new AuditLog(
                    "User",
                    adminUser.Id,
                    "BootstrapAdminCreated",
                    adminUser.Id,
                    "{}",
                    $$"""{"login":"{{options.Login}}"}""",
                    "Automatycznie utworzono konto administratora."),
                cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);
            return;
        }

        var assignedRole = await dbContext.UserRoles.AnyAsync(
            item => item.UserId == adminUser.Id && item.RoleId == adminRole.Id,
            cancellationToken);

        if (!assignedRole)
        {
            await dbContext.UserRoles.AddAsync(new UserRole(adminUser.Id, adminRole.Id), cancellationToken);
        }

        if (options.ResetPasswordOnStartup
            || string.IsNullOrWhiteSpace(adminUser.PasswordHash)
            || string.IsNullOrWhiteSpace(adminUser.PasswordSalt))
        {
            var (hash, salt) = passwordHasher.HashPassword(options.Password);
            adminUser.SetPassword(hash, salt, options.MustChangePassword);
        }

        adminUser.UpdateProfile(adminUser.UserName, adminUser.Email, adminUser.DisplayName, true);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
