using ErpSystem.Application.Modules.Auth;

namespace ErpSystem.Api.Features.Auth;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthFeatures(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/auth");

        group.MapPost("/login", async (LoginRequest request, IAuthService service, CancellationToken cancellationToken) =>
        {
            var response = await service.LoginAsync(request, cancellationToken);
            return response is null
                ? Results.Unauthorized()
                : Results.Ok(response);
        });

        return endpoints;
    }
}
