using System.Globalization;
using System.Text;

namespace ErpSystem.Desktop.Services;

public sealed class WarehouseWorkspaceStore
{
    private readonly ApiClient apiClient;
    private WarehouseWorkspaceSnapshot? snapshot;

    public WarehouseWorkspaceStore(ApiClient apiClient)
    {
        this.apiClient = apiClient;
    }

    public event EventHandler? Changed;

    public async Task<WarehouseWorkspaceSnapshot> GetSnapshotAsync(CancellationToken cancellationToken = default)
    {
        if (snapshot is not null)
        {
            return snapshot;
        }

        return await RefreshAsync(cancellationToken);
    }

    public async Task<WarehouseWorkspaceSnapshot> RefreshAsync(CancellationToken cancellationToken = default)
    {
        var referenceData = await apiClient.GetWarehouseReferenceDataAsync(cancellationToken);
        var stockItems = await apiClient.GetWarehouseStockAsync(cancellationToken: cancellationToken);
        var documentHeaders = await apiClient.GetWarehouseDocumentsAsync(cancellationToken);
        var details = await Task.WhenAll(documentHeaders.Select(item => apiClient.GetWarehouseDocumentAsync(item.Id, cancellationToken)));

        snapshot = new WarehouseWorkspaceSnapshot(
            referenceData,
            stockItems,
            details.OrderByDescending(item => item.DocumentDate).ThenByDescending(item => item.Id).ToArray());

        Changed?.Invoke(this, EventArgs.Empty);
        return snapshot;
    }

    public async Task<ApiClient.WarehouseDocumentDetailsSnapshot> CreateQuickDocumentAsync(
        string type,
        WarehouseQuickDocumentRequest request,
        CancellationToken cancellationToken = default)
    {
        var payload = new ApiClient.SaveWarehouseDocumentSnapshot(
            type,
            DateTime.Now,
            request.ContractorId,
            request.SourceWarehouseId,
            request.SourceLocationId,
            request.TargetWarehouseId,
            request.TargetLocationId,
            request.ExternalReference,
            request.Notes,
            [new ApiClient.SaveWarehouseDocumentPositionSnapshot(request.ProductId, request.Quantity, request.UnitPrice, request.SourceLocationId, request.TargetLocationId, request.PositionNotes)]);

        var created = await apiClient.CreateWarehouseDocumentAsync(payload, cancellationToken);
        await RefreshAsync(cancellationToken);
        return created;
    }

    public async Task<ApiClient.WarehouseDocumentDetailsSnapshot> UpdateDocumentAsync(
        WarehouseDocumentUpdateRequest request,
        CancellationToken cancellationToken = default)
    {
        var payload = new ApiClient.SaveWarehouseDocumentSnapshot(
            request.Type,
            request.DocumentDate,
            request.ContractorId,
            request.SourceWarehouseId,
            request.SourceLocationId,
            request.TargetWarehouseId,
            request.TargetLocationId,
            request.ExternalReference,
            request.Notes,
            request.Positions
                .Select(item => new ApiClient.SaveWarehouseDocumentPositionSnapshot(
                    item.ProductId,
                    item.Quantity,
                    item.UnitPrice,
                    item.SourceLocationId,
                    item.TargetLocationId,
                    item.Notes))
                .ToArray(),
            request.RowVersion);

        var updated = await apiClient.UpdateWarehouseDocumentAsync(request.Id, payload, cancellationToken);
        await RefreshAsync(cancellationToken);
        return updated;
    }

    public async Task<ApiClient.WarehouseDocumentDetailsSnapshot> ArchiveDocumentAsync(int documentId, byte[] rowVersion, CancellationToken cancellationToken = default)
    {
        var archived = await apiClient.ArchiveWarehouseDocumentAsync(documentId, new ApiClient.RowVersionSnapshot(rowVersion), cancellationToken);
        await RefreshAsync(cancellationToken);
        return archived;
    }

    public async Task DeleteDocumentAsync(int documentId, byte[] rowVersion, CancellationToken cancellationToken = default)
    {
        await apiClient.DeleteWarehouseDocumentAsync(documentId, new ApiClient.RowVersionSnapshot(rowVersion), cancellationToken);
        await RefreshAsync(cancellationToken);
    }

    public async Task<ApiClient.WarehouseProductSnapshot> UpdateProductAsync(
        WarehouseProductUpdateRequest request,
        CancellationToken cancellationToken = default)
    {
        var payload = new ApiClient.SaveWarehouseProductSnapshot(
            request.Code,
            request.Name,
            request.Sku,
            request.IsActive,
            request.MinimumStockLevel,
            request.ProductCategoryId,
            request.UnitOfMeasureId,
            request.RowVersion);

        var updated = await apiClient.UpdateWarehouseProductAsync(request.Id, payload, cancellationToken);
        await RefreshAsync(cancellationToken);
        return updated;
    }

    public async Task<ApiClient.WarehouseProductSnapshot> CreateProductAsync(
        WarehouseProductCreateRequest request,
        CancellationToken cancellationToken = default)
    {
        var payload = new ApiClient.SaveWarehouseProductSnapshot(
            request.Code,
            request.Name,
            request.Sku,
            request.IsActive,
            request.MinimumStockLevel,
            request.ProductCategoryId,
            request.UnitOfMeasureId,
            Array.Empty<byte>());

        var created = await apiClient.CreateWarehouseProductAsync(payload, cancellationToken);
        await RefreshAsync(cancellationToken);
        return created;
    }

    public async Task DeleteProductAsync(int productId, byte[] rowVersion, CancellationToken cancellationToken = default)
    {
        await apiClient.DeleteWarehouseProductAsync(productId, new ApiClient.RowVersionSnapshot(rowVersion), cancellationToken);
        await RefreshAsync(cancellationToken);
    }

    public string ExportDocumentsCsv(IEnumerable<WarehouseDocumentExportRow> rows)
    {
        var exportsDirectory = PrepareExportsDirectory();
        var filePath = Path.Combine(exportsDirectory, $"warehouse-documents-{DateTime.Now:yyyyMMdd-HHmmss}.csv");
        var builder = new StringBuilder();
        builder.AppendLine("Numer;Typ;Status;Magazyn;Kontrahent;Data;Pozycje;Ilosc;Wartosc");
        foreach (var row in rows)
        {
            builder.AppendLine(string.Join(";", Escape(row.Number), Escape(row.Type), Escape(row.Status), Escape(row.Warehouse), Escape(row.Counterparty), Escape(row.DocumentDate), Escape(row.PositionCount), Escape(row.Quantity), Escape(row.Value)));
        }

        File.WriteAllText(filePath, builder.ToString(), Encoding.UTF8);
        return filePath;
    }

    public string ExportGoodsCsv(IEnumerable<WarehouseGoodsExportRow> rows)
    {
        var exportsDirectory = PrepareExportsDirectory();
        var filePath = Path.Combine(exportsDirectory, $"warehouse-goods-{DateTime.Now:yyyyMMdd-HHmmss}.csv");
        var builder = new StringBuilder();
        builder.AppendLine("Kod;Nazwa;Kategoria;Jednostka;Stan;Minimum;Status;Dostawca");
        foreach (var row in rows)
        {
            builder.AppendLine(string.Join(";", Escape(row.Code), Escape(row.Name), Escape(row.Category), Escape(row.Unit), Escape(row.OnHand), Escape(row.Minimum), Escape(row.Status), Escape(row.Supplier)));
        }

        File.WriteAllText(filePath, builder.ToString(), Encoding.UTF8);
        return filePath;
    }

    private static string PrepareExportsDirectory()
    {
        var root = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ErpSystemExports");
        Directory.CreateDirectory(root);
        return root;
    }

    private static string Escape(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var escaped = value.Replace("\"", "\"\"");
        return escaped.Contains(';') || escaped.Contains('\n') || escaped.Contains('\r')
            ? $"\"{escaped}\""
            : escaped;
    }
}

public sealed record WarehouseWorkspaceSnapshot(
    ApiClient.WarehouseReferenceDataSnapshot ReferenceData,
    IReadOnlyList<ApiClient.WarehouseStockItemSnapshot> StockItems,
    IReadOnlyList<ApiClient.WarehouseDocumentDetailsSnapshot> Documents);

public sealed record WarehouseQuickDocumentRequest(
    int ProductId,
    decimal Quantity,
    decimal? UnitPrice,
    int? ContractorId,
    int? SourceWarehouseId,
    int? SourceLocationId,
    int? TargetWarehouseId,
    int? TargetLocationId,
    string? ExternalReference,
    string? Notes,
    string? PositionNotes);

public sealed record WarehouseDocumentUpdateRequest(
    int Id,
    string Type,
    DateTime DocumentDate,
    int? ContractorId,
    int? SourceWarehouseId,
    int? SourceLocationId,
    int? TargetWarehouseId,
    int? TargetLocationId,
    string? ExternalReference,
    string? Notes,
    IReadOnlyList<ApiClient.WarehouseDocumentPositionSnapshot> Positions,
    byte[] RowVersion);

public sealed record WarehouseProductUpdateRequest(
    int Id,
    string Code,
    string Name,
    string? Sku,
    bool IsActive,
    decimal MinimumStockLevel,
    int ProductCategoryId,
    int UnitOfMeasureId,
    byte[] RowVersion);

public sealed record WarehouseProductCreateRequest(
    string Code,
    string Name,
    string? Sku,
    bool IsActive,
    decimal MinimumStockLevel,
    int ProductCategoryId,
    int UnitOfMeasureId);

public sealed record WarehouseDocumentExportRow(
    string Number,
    string Type,
    string Status,
    string Warehouse,
    string Counterparty,
    string DocumentDate,
    string PositionCount,
    string Quantity,
    string Value);

public sealed record WarehouseGoodsExportRow(
    string Code,
    string Name,
    string Category,
    string Unit,
    string OnHand,
    string Minimum,
    string Status,
    string Supplier);
