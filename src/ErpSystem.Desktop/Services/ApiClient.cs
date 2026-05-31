using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ErpSystem.Desktop.Services;

public sealed class ApiClient
{
    private readonly HttpClient httpClient;
    private readonly SessionService? sessionService;

    public ApiClient(SessionService? sessionService = null, string? baseAddress = null)
    {
        this.sessionService = sessionService;
        httpClient = new HttpClient
        {
            BaseAddress = new Uri(ApiEndpointResolver.Resolve(baseAddress), UriKind.Absolute)
        };

        if (sessionService is not null)
        {
            sessionService.Changed += OnSessionChanged;
            ApplyAuthorizationHeader();
        }
    }

    public string BaseAddress => httpClient.BaseAddress?.ToString().TrimEnd('/') ?? string.Empty;

    public async Task<ApiStatusSnapshot> GetHealthAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var payload = await httpClient.GetFromJsonAsync<HealthResponse>("health", cancellationToken);
            if (payload is null)
            {
                return new ApiStatusSnapshot(false, "Brak odpowiedzi API", "Nie udalo sie pobrac statusu synchronizacji z API.", string.Empty, null);
            }

            return new ApiStatusSnapshot(
                true,
                "Połączono z API",
                $"API działa w środowisku {payload.Environment}.",
                payload.Environment,
                payload.Utc);
        }
        catch (Exception)
        {
            return new ApiStatusSnapshot(
                false,
                "Brak połączenia z API",
                "Nie udało się odświeżyć połączenia z API.",
                string.Empty,
                null);
        }
    }

    public async Task<DashboardSummarySnapshot> GetDashboardSummaryAsync(CancellationToken cancellationToken = default)
        => await GetRequiredAsync<DashboardSummarySnapshot>("api/dashboard/summary", cancellationToken);

    public async Task<WarehouseReferenceDataSnapshot> GetWarehouseReferenceDataAsync(CancellationToken cancellationToken = default)
        => await GetRequiredAsync<WarehouseReferenceDataSnapshot>("features/warehouse/reference-data", cancellationToken);

    public async Task<IReadOnlyList<WarehouseStockItemSnapshot>> GetWarehouseStockAsync(
        int? warehouseId = null,
        int? productId = null,
        CancellationToken cancellationToken = default)
    {
        var query = new List<string>();
        if (warehouseId.HasValue)
        {
            query.Add($"warehouseId={warehouseId.Value}");
        }

        if (productId.HasValue)
        {
            query.Add($"productId={productId.Value}");
        }

        var path = query.Count == 0
            ? "features/warehouse/stock"
            : $"features/warehouse/stock?{string.Join("&", query)}";

        return await GetRequiredAsync<WarehouseStockItemSnapshot[]>(path, cancellationToken);
    }

    public async Task<IReadOnlyList<WarehouseDocumentListItemSnapshot>> GetWarehouseDocumentsAsync(CancellationToken cancellationToken = default)
        => await GetRequiredAsync<WarehouseDocumentListItemSnapshot[]>("features/warehouse/documents", cancellationToken);

    public async Task<WarehouseDocumentDetailsSnapshot> GetWarehouseDocumentAsync(int documentId, CancellationToken cancellationToken = default)
        => await GetRequiredAsync<WarehouseDocumentDetailsSnapshot>($"features/warehouse/documents/{documentId}", cancellationToken);

    public async Task<WarehouseDocumentDetailsSnapshot> CreateWarehouseDocumentAsync(
        SaveWarehouseDocumentSnapshot request,
        CancellationToken cancellationToken = default)
        => await SendAndReadAsync<WarehouseDocumentDetailsSnapshot>(HttpMethod.Post, "features/warehouse/documents", request, cancellationToken);

    public async Task<WarehouseDocumentDetailsSnapshot> UpdateWarehouseDocumentAsync(
        int documentId,
        SaveWarehouseDocumentSnapshot request,
        CancellationToken cancellationToken = default)
        => await SendAndReadAsync<WarehouseDocumentDetailsSnapshot>(HttpMethod.Put, $"features/warehouse/documents/{documentId}", request, cancellationToken);

    public async Task<WarehouseDocumentDetailsSnapshot> PostWarehouseDocumentAsync(
        int documentId,
        RowVersionSnapshot request,
        CancellationToken cancellationToken = default)
        => await SendAndReadAsync<WarehouseDocumentDetailsSnapshot>(HttpMethod.Post, $"features/warehouse/documents/{documentId}/post", request, cancellationToken);

    public async Task<WarehouseDocumentDetailsSnapshot> ArchiveWarehouseDocumentAsync(
        int documentId,
        RowVersionSnapshot request,
        CancellationToken cancellationToken = default)
        => await SendAndReadAsync<WarehouseDocumentDetailsSnapshot>(HttpMethod.Post, $"features/warehouse/documents/{documentId}/archive", request, cancellationToken);

    public async Task DeleteWarehouseDocumentAsync(int documentId, RowVersionSnapshot request, CancellationToken cancellationToken = default)
        => await SendWithoutPayloadAsync(HttpMethod.Post, $"features/warehouse/documents/{documentId}/delete", request, cancellationToken);

    public async Task<IReadOnlyList<WarehouseProductSnapshot>> GetWarehouseProductsAsync(CancellationToken cancellationToken = default)
        => await GetRequiredAsync<WarehouseProductSnapshot[]>("features/warehouse/products", cancellationToken);

    public async Task<WarehouseProductSnapshot> CreateWarehouseProductAsync(
        SaveWarehouseProductSnapshot request,
        CancellationToken cancellationToken = default)
        => await SendAndReadAsync<WarehouseProductSnapshot>(HttpMethod.Post, "features/warehouse/products", request, cancellationToken);

    public async Task<WarehouseProductSnapshot> UpdateWarehouseProductAsync(
        int productId,
        SaveWarehouseProductSnapshot request,
        CancellationToken cancellationToken = default)
        => await SendAndReadAsync<WarehouseProductSnapshot>(HttpMethod.Put, $"features/warehouse/products/{productId}", request, cancellationToken);

    public async Task DeleteWarehouseProductAsync(int productId, RowVersionSnapshot request, CancellationToken cancellationToken = default)
        => await SendWithoutPayloadAsync(HttpMethod.Post, $"features/warehouse/products/{productId}/delete", request, cancellationToken);

    public sealed record ApiStatusSnapshot(bool IsOnline, string Label, string Description, string EnvironmentName, DateTime? UtcTimestamp);

    public sealed record DashboardSummarySnapshot(
        int ProductCount,
        int DraftWarehouseDocumentCount,
        int TransportOrderCount,
        int ActiveEmployeeCount);

    public sealed record WarehouseReferenceDataSnapshot(
        IReadOnlyList<WarehouseOptionSnapshot> Warehouses,
        IReadOnlyList<WarehouseLocationSnapshot> Locations,
        IReadOnlyList<ProductCategorySnapshot> Categories,
        IReadOnlyList<UnitOfMeasureSnapshot> UnitsOfMeasure,
        IReadOnlyList<WarehouseProductSnapshot> Products,
        IReadOnlyList<WarehouseDocumentTypeOptionSnapshot> DocumentTypes);

    public sealed record WarehouseOptionSnapshot(int Id, string Code, string Name);
    public sealed record WarehouseLocationSnapshot(int Id, int WarehouseId, string Code, string Name, bool IsActive);
    public sealed record ProductCategorySnapshot(int Id, string Code, string Name);
    public sealed record UnitOfMeasureSnapshot(int Id, string Code, string Name, string Symbol, int DecimalPrecision);

    public sealed record WarehouseProductSnapshot(
        int Id,
        string Code,
        string Name,
        string? Sku,
        bool IsActive,
        decimal MinimumStockLevel,
        int ProductCategoryId,
        string ProductCategoryCode,
        string ProductCategoryName,
        int UnitOfMeasureId,
        string UnitOfMeasureCode,
        string UnitOfMeasureSymbol,
        DateTime LastUpdatedAtUtc,
        byte[] RowVersion);

    public sealed record WarehouseStockItemSnapshot(
        int Id,
        int WarehouseId,
        string WarehouseCode,
        string WarehouseName,
        int? WarehouseLocationId,
        string? WarehouseLocationCode,
        string? WarehouseLocationName,
        int ProductId,
        string ProductCode,
        string ProductName,
        string UnitOfMeasureSymbol,
        decimal QuantityOnHand,
        DateTime? LastMovementAtUtc);

    public sealed record WarehouseDocumentListItemSnapshot(
        int Id,
        string Number,
        string Type,
        string Status,
        DateTime DocumentDate,
        int? ContractorId,
        string? ContractorName,
        int? SourceWarehouseId,
        string? SourceWarehouseCode,
        int? TargetWarehouseId,
        string? TargetWarehouseCode,
        int PositionCount,
        byte[] RowVersion);

    public sealed record WarehouseDocumentDetailsSnapshot(
        int Id,
        string Number,
        string Type,
        string Status,
        DateTime DocumentDate,
        int? ContractorId,
        string? ContractorName,
        int? SourceWarehouseId,
        string? SourceWarehouseCode,
        int? SourceLocationId,
        string? SourceLocationCode,
        int? TargetWarehouseId,
        string? TargetWarehouseCode,
        int? TargetLocationId,
        string? TargetLocationCode,
        string? ExternalReference,
        string? Notes,
        DateTime? PostedAtUtc,
        int? PostedByUserId,
        IReadOnlyList<WarehouseDocumentPositionSnapshot> Positions,
        IReadOnlyList<StockMovementSnapshot> Movements,
        byte[] RowVersion);

    public sealed record WarehouseDocumentPositionSnapshot(
        int Id,
        int ProductId,
        string ProductCode,
        string ProductName,
        decimal Quantity,
        decimal? UnitPrice,
        int? SourceLocationId,
        string? SourceLocationCode,
        string? SourceLocationName,
        int? TargetLocationId,
        string? TargetLocationCode,
        string? TargetLocationName,
        string? Notes);

    public sealed record StockMovementSnapshot(
        int Id,
        int ProductId,
        string ProductCode,
        string ProductName,
        int WarehouseId,
        string WarehouseCode,
        string WarehouseName,
        int? WarehouseLocationId,
        string? WarehouseLocationCode,
        string? WarehouseLocationName,
        decimal QuantityDelta,
        decimal? UnitPrice,
        DateTime MovementDateUtc);

    public sealed record WarehouseDocumentTypeOptionSnapshot(
        string Code,
        string Label,
        bool RequiresSourceWarehouse,
        bool RequiresTargetWarehouse,
        bool UsesInventoryAdjustment);

    public sealed record SaveWarehouseDocumentSnapshot(
        string Type,
        DateTime DocumentDate,
        int? ContractorId,
        int? SourceWarehouseId,
        int? SourceLocationId,
        int? TargetWarehouseId,
        int? TargetLocationId,
        string? ExternalReference,
        string? Notes,
        IReadOnlyList<SaveWarehouseDocumentPositionSnapshot> Positions,
        byte[]? RowVersion = null);

    public sealed record SaveWarehouseDocumentPositionSnapshot(
        int ProductId,
        decimal Quantity,
        decimal? UnitPrice,
        int? SourceLocationId,
        int? TargetLocationId,
        string? Notes);

    public sealed record SaveWarehouseProductSnapshot(
        string Code,
        string Name,
        string? Sku,
        bool IsActive,
        decimal MinimumStockLevel,
        int ProductCategoryId,
        int UnitOfMeasureId,
        byte[] RowVersion);

    public sealed record RowVersionSnapshot(byte[] RowVersion);

    private sealed record HealthResponse(string Status, string Environment, DateTime Utc);

    private void OnSessionChanged(object? sender, EventArgs e)
    {
        ApplyAuthorizationHeader();
    }

    private void ApplyAuthorizationHeader()
    {
        httpClient.DefaultRequestHeaders.Authorization = string.IsNullOrWhiteSpace(sessionService?.Token)
            ? null
            : new AuthenticationHeaderValue("Bearer", sessionService.Token);
    }

    private async Task<T> GetRequiredAsync<T>(string path, CancellationToken cancellationToken)
    {
        var payload = await httpClient.GetFromJsonAsync<T>(path, cancellationToken);
        if (payload is null)
        {
            throw new InvalidOperationException($"Brak danych z endpointu '{path}'.");
        }

        return payload;
    }

    private async Task<T> SendAndReadAsync<T>(HttpMethod method, string path, object payload, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(method, path)
        {
            Content = JsonContent.Create(payload)
        };

        using var response = await httpClient.SendAsync(request, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        var result = await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
        if (result is null)
        {
            throw new InvalidOperationException($"Endpoint '{path}' zwrocil pusty wynik.");
        }

        return result;
    }

    private async Task SendWithoutPayloadAsync(HttpMethod method, string path, object payload, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(method, path)
        {
            Content = JsonContent.Create(payload)
        };

        using var response = await httpClient.SendAsync(request, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var rawError = await response.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(rawError))
        {
            response.EnsureSuccessStatusCode();
        }

        throw new InvalidOperationException(rawError);
    }
}
