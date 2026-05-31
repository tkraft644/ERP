using ErpSystem.Domain.Modules.System.Identity;

namespace ErpSystem.Application.Modules.Auth;

public interface IJwtTokenFactory
{
    string CreateToken(User user, IReadOnlyList<string> roles, IReadOnlyList<string> permissions);
}
