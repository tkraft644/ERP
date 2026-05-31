namespace ErpSystem.Application.Modules.Warehouse;

public interface IWarehouseService
{
    Task<WarehouseReferenceDataView> GetReferenceDataAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StockItemView>> GetStockAsync(int? warehouseId, int? productId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WarehouseDocumentListItemView>> GetDocumentsAsync(CancellationToken cancellationToken = default);
    Task<WarehouseDocumentDetailsView?> GetDocumentAsync(int documentId, CancellationToken cancellationToken = default);
    Task<WarehouseDocumentDetailsView> CreateDocumentAsync(SaveWarehouseDocumentRequest request, CancellationToken cancellationToken = default);
    Task<WarehouseDocumentDetailsView?> UpdateDocumentAsync(int documentId, SaveWarehouseDocumentRequest request, CancellationToken cancellationToken = default);
    Task<WarehouseDocumentDetailsView?> PostDocumentAsync(int documentId, PostWarehouseDocumentRequest request, CancellationToken cancellationToken = default);
    Task<WarehouseDocumentDetailsView?> ArchiveDocumentAsync(int documentId, RowVersionRequest request, CancellationToken cancellationToken = default);
    Task DeleteDocumentAsync(int documentId, RowVersionRequest request, CancellationToken cancellationToken = default);
    Task<ProductView> CreateProductAsync(SaveWarehouseProductRequest request, CancellationToken cancellationToken = default);
    Task<ProductView?> UpdateProductAsync(int productId, SaveWarehouseProductRequest request, CancellationToken cancellationToken = default);
    Task DeleteProductAsync(int productId, RowVersionRequest request, CancellationToken cancellationToken = default);
    IReadOnlyList<WarehouseDocumentTypeOptionView> GetDocumentTypes();
}
