using ErpSystem.Application.Common;
using ErpSystem.Application.Modules.Warehouse;
using ErpSystem.Domain.Modules.System.Auditing;
using ErpSystem.Domain.Modules.System.Documents;
using ErpSystem.Domain.Modules.Warehouse;
using ErpSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ErpSystem.Infrastructure.Modules.Warehouse;

public sealed class EfWarehouseRepository : IWarehouseRepository
{
    private readonly ErpSystemDbContext dbContext;

    public EfWarehouseRepository(ErpSystemDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Domain.Modules.Warehouse.Warehouse>> GetWarehousesAsync(CancellationToken cancellationToken = default)
        => await dbContext.Warehouses.AsNoTracking().OrderBy(item => item.Name).ToArrayAsync(cancellationToken);

    public async Task<IReadOnlyList<WarehouseLocation>> GetLocationsAsync(CancellationToken cancellationToken = default)
        => await dbContext.WarehouseLocations.AsNoTracking().OrderBy(item => item.Name).ToArrayAsync(cancellationToken);

    public async Task<IReadOnlyList<ProductCategory>> GetCategoriesAsync(CancellationToken cancellationToken = default)
        => await dbContext.ProductCategories.AsNoTracking().OrderBy(item => item.Name).ToArrayAsync(cancellationToken);

    public async Task<IReadOnlyList<UnitOfMeasure>> GetUnitsAsync(CancellationToken cancellationToken = default)
        => await dbContext.UnitsOfMeasure.AsNoTracking().OrderBy(item => item.Name).ToArrayAsync(cancellationToken);

    public async Task<IReadOnlyList<Product>> GetProductsAsync(bool includeInactive, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Products
            .AsNoTracking()
            .Include(item => item.ProductCategory)
            .Include(item => item.UnitOfMeasure)
            .AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(item => item.IsActive);
        }

        return await query.OrderBy(item => item.Name).ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StockItem>> GetStockItemsAsync(int? warehouseId, int? productId, CancellationToken cancellationToken = default)
    {
        var query = dbContext.StockItems
            .AsNoTracking()
            .Include(item => item.Warehouse)
            .Include(item => item.WarehouseLocation)
            .Include(item => item.Product)
                .ThenInclude(item => item.UnitOfMeasure)
            .AsQueryable();

        if (warehouseId.HasValue)
        {
            query = query.Where(item => item.WarehouseId == warehouseId.Value);
        }

        if (productId.HasValue)
        {
            query = query.Where(item => item.ProductId == productId.Value);
        }

        return await query.OrderBy(item => item.Id).ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<WarehouseDocument>> GetDocumentsAsync(CancellationToken cancellationToken = default)
        => await dbContext.WarehouseDocuments
            .AsNoTracking()
            .Include(item => item.Contractor)
            .Include(item => item.SourceWarehouse)
            .Include(item => item.TargetWarehouse)
            .Include(item => item.Positions)
            .OrderByDescending(item => item.DocumentDate)
            .ToArrayAsync(cancellationToken);

    public Task<WarehouseDocument?> GetDocumentAsync(int documentId, CancellationToken cancellationToken = default)
        => BuildDocumentQuery(asNoTracking: true).FirstOrDefaultAsync(item => item.Id == documentId, cancellationToken);

    public Task<WarehouseDocument?> GetDocumentForUpdateAsync(int documentId, CancellationToken cancellationToken = default)
        => BuildDocumentQuery(asNoTracking: false).FirstOrDefaultAsync(item => item.Id == documentId, cancellationToken);

    public Task AddDocumentAsync(WarehouseDocument document, CancellationToken cancellationToken = default)
        => dbContext.WarehouseDocuments.AddAsync(document, cancellationToken).AsTask();

    public Task AddProductAsync(Product product, CancellationToken cancellationToken = default)
        => dbContext.Products.AddAsync(product, cancellationToken).AsTask();

    public Task AddStockMovementAsync(StockMovement movement, CancellationToken cancellationToken = default)
        => dbContext.StockMovements.AddAsync(movement, cancellationToken).AsTask();

    public Task AddAuditLogAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
        => dbContext.AuditLogs.AddAsync(auditLog, cancellationToken).AsTask();

    public Task<StockItem?> GetStockItemForUpdateAsync(int warehouseId, int? warehouseLocationId, int productId, CancellationToken cancellationToken = default)
        => dbContext.StockItems.FirstOrDefaultAsync(
            item => item.WarehouseId == warehouseId &&
                    item.WarehouseLocationId == warehouseLocationId &&
                    item.ProductId == productId,
            cancellationToken);

    public Task<Product?> GetProductAsync(int productId, CancellationToken cancellationToken = default)
        => dbContext.Products
            .AsNoTracking()
            .Include(item => item.ProductCategory)
            .Include(item => item.UnitOfMeasure)
            .FirstOrDefaultAsync(item => item.Id == productId, cancellationToken);

    public Task<Product?> GetProductForUpdateAsync(int productId, CancellationToken cancellationToken = default)
        => dbContext.Products
            .Include(item => item.ProductCategory)
            .Include(item => item.UnitOfMeasure)
            .FirstOrDefaultAsync(item => item.Id == productId, cancellationToken);

    public Task<string> GenerateDocumentNumberAsync(string key, DateTime utcNow, CancellationToken cancellationToken = default)
        => GenerateDocumentNumberInternalAsync(key, utcNow, cancellationToken);

    public void AddStockItem(StockItem stockItem)
    {
        dbContext.StockItems.Add(stockItem);
    }

    public void RemoveDocument(WarehouseDocument document)
    {
        dbContext.WarehouseDocuments.Remove(document);
    }

    public void RemoveProduct(Product product)
    {
        dbContext.Products.Remove(product);
    }

    public void SetOriginalRowVersion(WarehouseDocument document, byte[] rowVersion)
    {
        dbContext.Entry(document).Property(item => item.RowVersion).OriginalValue = rowVersion;
    }

    public void SetOriginalRowVersion(Product product, byte[] rowVersion)
    {
        dbContext.Entry(product).Property(item => item.RowVersion).OriginalValue = rowVersion;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            throw new ConcurrencyConflictException("The warehouse document was modified by another user. Refresh the data and try again.", exception);
        }
    }

    private IQueryable<WarehouseDocument> BuildDocumentQuery(bool asNoTracking)
    {
        var query = dbContext.WarehouseDocuments
            .Include(item => item.Contractor)
            .Include(item => item.SourceWarehouse)
            .Include(item => item.SourceLocation)
            .Include(item => item.TargetWarehouse)
            .Include(item => item.TargetLocation)
            .Include(item => item.Positions)
                .ThenInclude(item => item.Product)
            .Include(item => item.Positions)
                .ThenInclude(item => item.SourceLocation)
            .Include(item => item.Positions)
                .ThenInclude(item => item.TargetLocation)
            .Include(item => item.StockMovements)
                .ThenInclude(item => item.Product)
            .Include(item => item.StockMovements)
                .ThenInclude(item => item.Warehouse)
            .Include(item => item.StockMovements)
                .ThenInclude(item => item.WarehouseLocation)
            .AsQueryable();

        return asNoTracking ? query.AsNoTracking() : query;
    }

    private async Task<string> GenerateDocumentNumberInternalAsync(string key, DateTime utcNow, CancellationToken cancellationToken)
    {
        var sequence = await dbContext.DocumentNumberSequences.FirstOrDefaultAsync(item => item.Key == key, cancellationToken);
        if (sequence is null)
        {
            throw new InvalidOperationException($"Document sequence '{key}' was not found.");
        }

        var nextNumber = sequence.GenerateNextNumber(utcNow);
        return nextNumber;
    }
}
