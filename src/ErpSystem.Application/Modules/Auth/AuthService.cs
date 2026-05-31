namespace ErpSystem.Application.Modules.Auth;

public sealed class AuthService : IAuthService
{
    private readonly IAuthRepository repository;
    private readonly IPasswordHasher passwordHasher;
    private readonly IJwtTokenFactory jwtTokenFactory;

    public AuthService(IAuthRepository repository, IPasswordHasher passwordHasher, IJwtTokenFactory jwtTokenFactory)
    {
        this.repository = repository;
        this.passwordHasher = passwordHasher;
        this.jwtTokenFactory = jwtTokenFactory;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var login = request.Login.Trim();
        var password = request.Password.Trim();

        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        var user = await repository.FindByLoginAsync(login, cancellationToken);
        if (user is null || !user.IsActive)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(user.PasswordHash) || string.IsNullOrWhiteSpace(user.PasswordSalt))
        {
            return null;
        }

        if (!passwordHasher.VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
        {
            return null;
        }

        var roles = await repository.GetRolesForUserAsync(user.Id, cancellationToken);
        var permissions = await repository.GetPermissionsForUserAsync(user.Id, cancellationToken);
        var roleNames = roles
            .Select(item => item.Name)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(item => item)
            .ToArray();
        var permissionNames = permissions
            .Select(item => item.Name)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(item => item)
            .ToArray();

        var token = jwtTokenFactory.CreateToken(user, roleNames, permissionNames);

        return new LoginResponse(
            token,
            user.Id,
            user.UserName,
            user.DisplayName,
            user.MustChangePassword,
            roleNames,
            permissionNames);
    }
}
