using ErpSystem.Application.Modules.Auth;
using ErpSystem.Domain.Modules.System.Identity;
using ErpSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ErpSystem.Infrastructure.Modules.Auth;

public sealed class EfAuthRepository : IAuthRepository
{
    private readonly ErpSystemDbContext dbContext;

    public EfAuthRepository(ErpSystemDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Task<User?> FindByLoginAsync(string login, CancellationToken cancellationToken = default)
    {
        return dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                item => item.UserName == login || item.Email == login,
                cancellationToken);
    }

    public Task<IReadOnlyList<Role>> GetRolesForUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        return dbContext.UserRoles
            .AsNoTracking()
            .Where(item => item.UserId == userId)
            .Join(
                dbContext.Roles.AsNoTracking(),
                userRole => userRole.RoleId,
                role => role.Id,
                (_, role) => role)
            .OrderBy(item => item.Name)
            .ToArrayAsync(cancellationToken)
            .ContinueWith<IReadOnlyList<Role>>(task => task.Result, cancellationToken);
    }

    public Task<IReadOnlyList<Permission>> GetPermissionsForUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        return dbContext.UserRoles
            .AsNoTracking()
            .Where(item => item.UserId == userId)
            .Join(
                dbContext.RolePermissions.AsNoTracking(),
                userRole => userRole.RoleId,
                rolePermission => rolePermission.RoleId,
                (_, rolePermission) => rolePermission.PermissionId)
            .Distinct()
            .Join(
                dbContext.Permissions.AsNoTracking(),
                permissionId => permissionId,
                permission => permission.Id,
                (_, permission) => permission)
            .OrderBy(item => item.Name)
            .ToArrayAsync(cancellationToken)
            .ContinueWith<IReadOnlyList<Permission>>(task => task.Result, cancellationToken);
    }
}
