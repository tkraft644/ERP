using ErpSystem.Domain.Modules.System.Auditing;
using ErpSystem.Domain.Modules.Warehouse;

namespace ErpSystem.Application.Modules.Warehouse;

public interface IWarehouseRepository
{
    Task<IReadOnlyList<Domain.Modules.Warehouse.Warehouse>> GetWarehousesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WarehouseLocation>> GetLocationsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductCategory>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UnitOfMeasure>> GetUnitsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetProductsAsync(bool includeInactive, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StockItem>> GetStockItemsAsync(int? warehouseId, int? productId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WarehouseDocument>> GetDocumentsAsync(CancellationToken cancellationToken = default);
    Task<WarehouseDocument?> GetDocumentAsync(int documentId, CancellationToken cancellationToken = default);
    Task<WarehouseDocument?> GetDocumentForUpdateAsync(int documentId, CancellationToken cancellationToken = default);
    Task AddDocumentAsync(WarehouseDocument document, CancellationToken cancellationToken = default);
    Task AddProductAsync(Product product, CancellationToken cancellationToken = default);
    Task AddStockMovementAsync(StockMovement movement, CancellationToken cancellationToken = default);
    Task AddAuditLogAsync(AuditLog auditLog, CancellationToken cancellationToken = default);
    Task<StockItem?> GetStockItemForUpdateAsync(int warehouseId, int? warehouseLocationId, int productId, CancellationToken cancellationToken = default);
    Task<Product?> GetProductAsync(int productId, CancellationToken cancellationToken = default);
    Task<Product?> GetProductForUpdateAsync(int productId, CancellationToken cancellationToken = default);
    Task<string> GenerateDocumentNumberAsync(string key, DateTime utcNow, CancellationToken cancellationToken = default);
    void AddStockItem(StockItem stockItem);
    void RemoveDocument(WarehouseDocument document);
    void RemoveProduct(Product product);
    void SetOriginalRowVersion(WarehouseDocument document, byte[] rowVersion);
    void SetOriginalRowVersion(Product product, byte[] rowVersion);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
