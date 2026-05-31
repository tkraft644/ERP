namespace ErpSystem.Desktop.Services;

public static class ApiEndpointResolver
{
    public const string ApiBaseUrlEnvironmentVariable = "ERP_API_BASE_URL";
    public const string DefaultApiBaseAddress = "http://localhost:5180/";

    public static string Resolve(string? baseAddress = null)
    {
        var candidate = string.IsNullOrWhiteSpace(baseAddress)
            ? Environment.GetEnvironmentVariable(ApiBaseUrlEnvironmentVariable)
            : baseAddress;

        if (string.IsNullOrWhiteSpace(candidate))
        {
            return DefaultApiBaseAddress;
        }

        if (!Uri.TryCreate(candidate, UriKind.Absolute, out var uri))
        {
            return DefaultApiBaseAddress;
        }

        return uri.AbsoluteUri.EndsWith("/", StringComparison.Ordinal) ? uri.AbsoluteUri : $"{uri.AbsoluteUri}/";
    }
}
