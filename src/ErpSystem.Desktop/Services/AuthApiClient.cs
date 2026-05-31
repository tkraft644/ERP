using System.Net;
using System.Net.Http.Json;

namespace ErpSystem.Desktop.Services;

public sealed class AuthApiClient
{
    private readonly HttpClient httpClient;

    public AuthApiClient(string? baseAddress = null)
    {
        httpClient = new HttpClient
        {
            BaseAddress = new Uri(ApiEndpointResolver.Resolve(baseAddress), UriKind.Absolute)
        };
    }

    public string BaseAddress => httpClient.BaseAddress?.ToString().TrimEnd('/') ?? string.Empty;

    public async Task<ApiConnectionSnapshot> GetHealthAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var payload = await httpClient.GetFromJsonAsync<HealthResponse>("health", cancellationToken);
            if (payload is null)
            {
                return new ApiConnectionSnapshot(false, "Brak odpowiedzi API", "API nie zwróciło danych statusowych.", string.Empty, null);
            }

            return new ApiConnectionSnapshot(
                true,
                "Połączono z API",
                $"API działa w środowisku {payload.Environment}.",
                payload.Environment,
                payload.Utc);
        }
        catch (Exception)
        {
            return new ApiConnectionSnapshot(
                false,
                "Brak połączenia z API",
                "Nie udało się połączyć z lokalnym API. Sprawdź, czy działa ErpSystem.Api i SQL Server.",
                string.Empty,
                null);
        }
    }

    public async Task<AuthenticatedSession?> LoginAsync(string login, string password, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "api/auth/login",
            new LoginRequestPayload(login, password),
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<LoginResponsePayload>(cancellationToken: cancellationToken);
        if (payload is null)
        {
            throw new InvalidOperationException("API zwróciło pustą odpowiedź logowania.");
        }

        return new AuthenticatedSession(
            payload.Token,
            payload.UserId,
            payload.UserName,
            payload.DisplayName,
            payload.MustChangePassword,
            payload.Roles ?? [],
            payload.Permissions ?? []);
    }

    public sealed record ApiConnectionSnapshot(
        bool IsOnline,
        string Label,
        string Description,
        string EnvironmentName,
        DateTime? UtcTimestamp);

    private sealed record LoginRequestPayload(string Login, string Password);

    private sealed record LoginResponsePayload(
        string Token,
        int UserId,
        string UserName,
        string DisplayName,
        bool MustChangePassword,
        IReadOnlyList<string>? Roles,
        IReadOnlyList<string>? Permissions);

    private sealed record HealthResponse(string Status, string Environment, DateTime Utc);
}
