using ErpSystem.Domain.Modules.System.Identity;

namespace ErpSystem.Application.Modules.Auth;

public interface IAuthRepository
{
    Task<User?> FindByLoginAsync(string login, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Role>> GetRolesForUserAsync(int userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Permission>> GetPermissionsForUserAsync(int userId, CancellationToken cancellationToken = default);
}
