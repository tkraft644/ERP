using System.Text.Json;
using ErpSystem.Application.Common;
using ErpSystem.Domain.Modules.System.Auditing;
using ErpSystem.Domain.Modules.Warehouse;

namespace ErpSystem.Application.Modules.Warehouse;

public sealed class WarehouseService : IWarehouseService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly IWarehouseRepository repository;
    private readonly ICurrentUserAccessor currentUserAccessor;

    public WarehouseService(IWarehouseRepository repository, ICurrentUserAccessor currentUserAccessor)
    {
        this.repository = repository;
        this.currentUserAccessor = currentUserAccessor;
    }

    public async Task<WarehouseReferenceDataView> GetReferenceDataAsync(CancellationToken cancellationToken = default)
    {
        var warehouses = await repository.GetWarehousesAsync(cancellationToken);
        var locations = await repository.GetLocationsAsync(cancellationToken);
        var categories = await repository.GetCategoriesAsync(cancellationToken);
        var units = await repository.GetUnitsAsync(cancellationToken);
        var products = await repository.GetProductsAsync(includeInactive: true, cancellationToken);

        return new WarehouseReferenceDataView(
            warehouses.OrderBy(item => item.Name).Select(item => new WarehouseOptionView(item.Id, item.Code, item.Name)).ToArray(),
            locations.OrderBy(item => item.Name).Select(item => new WarehouseLocationView(item.Id, item.WarehouseId, item.Code, item.Name, item.IsActive)).ToArray(),
            categories.OrderBy(item => item.Name).Select(item => new ProductCategoryView(item.Id, item.Code, item.Name)).ToArray(),
            units.OrderBy(item => item.Name).Select(item => new UnitOfMeasureView(item.Id, item.Code, item.Name, item.Symbol, item.DecimalPrecision)).ToArray(),
            products.OrderBy(item => item.Name).Select(MapProduct).ToArray(),
            GetDocumentTypes());
    }

    public async Task<IReadOnlyList<StockItemView>> GetStockAsync(int? warehouseId, int? productId, CancellationToken cancellationToken = default)
    {
        var items = await repository.GetStockItemsAsync(warehouseId, productId, cancellationToken);
        return items
            .OrderBy(item => item.Warehouse.Name)
            .ThenBy(item => item.Product.Name)
            .Select(MapStockItem)
            .ToArray();
    }

    public async Task<IReadOnlyList<WarehouseDocumentListItemView>> GetDocumentsAsync(CancellationToken cancellationToken = default)
    {
        var documents = await repository.GetDocumentsAsync(cancellationToken);
        return documents
            .OrderByDescending(item => item.DocumentDate)
            .ThenByDescending(item => item.Id)
            .Select(MapDocumentListItem)
            .ToArray();
    }

    public async Task<WarehouseDocumentDetailsView?> GetDocumentAsync(int documentId, CancellationToken cancellationToken = default)
    {
        var document = await repository.GetDocumentAsync(documentId, cancellationToken);
        return document is null ? null : MapDocumentDetails(document);
    }

    public async Task<WarehouseDocumentDetailsView> CreateDocumentAsync(SaveWarehouseDocumentRequest request, CancellationToken cancellationToken = default)
    {
        var type = ParseDocumentType(request.Type);
        ValidateDocumentRequest(type, request);

        var number = await repository.GenerateDocumentNumberAsync(GetSequenceKey(type), DateTime.UtcNow, cancellationToken);
        var document = new WarehouseDocument(
            number,
            type,
            request.DocumentDate,
            request.ContractorId,
            request.SourceWarehouseId,
            request.SourceLocationId,
            request.TargetWarehouseId,
            request.TargetLocationId,
            NormalizeOptional(request.ExternalReference),
            NormalizeOptional(request.Notes));
        document.ReplacePositions(MapPositions(request.Positions));

        await repository.AddDocumentAsync(document, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        var createdDocument = await repository.GetDocumentAsync(document.Id, cancellationToken)
            ?? throw new InvalidOperationException("Created warehouse document could not be reloaded.");

        await repository.AddAuditLogAsync(CreateAuditLog(
            createdDocument.Id,
            "Created",
            "{}",
            Serialize(MapDocumentDetails(createdDocument)),
            $"Created warehouse document '{createdDocument.Number}'."), cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return MapDocumentDetails(createdDocument);
    }

    public async Task<WarehouseDocumentDetailsView?> UpdateDocumentAsync(int documentId, SaveWarehouseDocumentRequest request, CancellationToken cancellationToken = default)
    {
        if (request.RowVersion is null || request.RowVersion.Length == 0)
        {
            throw new ArgumentException("RowVersion is required when updating a warehouse document.", nameof(request));
        }

        var document = await repository.GetDocumentForUpdateAsync(documentId, cancellationToken);
        if (document is null)
        {
            return null;
        }

        if (document.Status != WarehouseDocumentStatus.Draft)
        {
            throw new InvalidOperationException("Only draft warehouse documents can be edited.");
        }

        var type = ParseDocumentType(request.Type);
        ValidateDocumentRequest(type, request);

        repository.SetOriginalRowVersion(document, request.RowVersion);
        var previousView = MapDocumentDetails(document);

        document.Update(
            type,
            request.DocumentDate,
            request.ContractorId,
            request.SourceWarehouseId,
            request.SourceLocationId,
            request.TargetWarehouseId,
            request.TargetLocationId,
            NormalizeOptional(request.ExternalReference),
            NormalizeOptional(request.Notes));
        document.ReplacePositions(MapPositions(request.Positions));

        await repository.SaveChangesAsync(cancellationToken);
        var updatedDocument = await repository.GetDocumentAsync(document.Id, cancellationToken)
            ?? throw new InvalidOperationException("Updated warehouse document could not be reloaded.");
        await repository.AddAuditLogAsync(CreateAuditLog(
            updatedDocument.Id,
            "Updated",
            Serialize(previousView),
            Serialize(MapDocumentDetails(updatedDocument)),
            $"Updated warehouse document '{updatedDocument.Number}'."), cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return MapDocumentDetails(updatedDocument);
    }

    public async Task<WarehouseDocumentDetailsView?> PostDocumentAsync(int documentId, PostWarehouseDocumentRequest request, CancellationToken cancellationToken = default)
    {
        if (request.RowVersion.Length == 0)
        {
            throw new ArgumentException("RowVersion is required when posting a warehouse document.", nameof(request));
        }

        var document = await repository.GetDocumentForUpdateAsync(documentId, cancellationToken);
        if (document is null)
        {
            return null;
        }

        if (document.Status != WarehouseDocumentStatus.Draft)
        {
            throw new InvalidOperationException("Only draft warehouse documents can be posted.");
        }

        repository.SetOriginalRowVersion(document, request.RowVersion);

        var movementDateUtc = DateTime.UtcNow;
        foreach (var position in document.Positions)
        {
            if (position.Quantity < 0m || (position.Quantity == 0m && document.Type != WarehouseDocumentType.INW))
            {
                throw new InvalidOperationException("Document position quantity must be greater than zero. Inventory documents may set quantity to zero.");
            }

            foreach (var movement in await BuildMovementsAsync(document, position, movementDateUtc, cancellationToken))
            {
                var stockItem = await repository.GetStockItemForUpdateAsync(
                    movement.WarehouseId,
                    movement.WarehouseLocationId,
                    movement.ProductId,
                    cancellationToken);

                stockItem ??= CreateStockItem(movement);
                var resultingQuantity = stockItem.QuantityOnHand + movement.QuantityDelta;
                if (resultingQuantity < 0m)
                {
                    throw new InvalidOperationException(
                        $"Posting document '{document.Number}' would produce negative stock for product '{position.ProductId}'.");
                }

                stockItem.ApplyMovement(movement.QuantityDelta, movementDateUtc);
                await repository.AddStockMovementAsync(movement, cancellationToken);
            }
        }

        document.MarkAsPosted(currentUserAccessor.UserId, movementDateUtc);
        await repository.SaveChangesAsync(cancellationToken);
        var postedDocument = await repository.GetDocumentAsync(document.Id, cancellationToken)
            ?? throw new InvalidOperationException("Posted warehouse document could not be reloaded.");
        await repository.AddAuditLogAsync(CreateAuditLog(
            postedDocument.Id,
            "Posted",
            "{}",
            Serialize(MapDocumentDetails(postedDocument)),
            $"Posted warehouse document '{postedDocument.Number}'."), cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return MapDocumentDetails(postedDocument);
    }

    public async Task<WarehouseDocumentDetailsView?> ArchiveDocumentAsync(int documentId, RowVersionRequest request, CancellationToken cancellationToken = default)
    {
        if (request.RowVersion.Length == 0)
        {
            throw new ArgumentException("RowVersion is required when archiving a warehouse document.", nameof(request));
        }

        var document = await repository.GetDocumentForUpdateAsync(documentId, cancellationToken);
        if (document is null)
        {
            return null;
        }

        if (document.Status == WarehouseDocumentStatus.Cancelled)
        {
            return MapDocumentDetails(document);
        }

        repository.SetOriginalRowVersion(document, request.RowVersion);
        var previousView = MapDocumentDetails(document);
        document.Archive();

        await repository.SaveChangesAsync(cancellationToken);
        var archivedDocument = await repository.GetDocumentAsync(document.Id, cancellationToken)
            ?? throw new InvalidOperationException("Archived warehouse document could not be reloaded.");
        await repository.AddAuditLogAsync(CreateAuditLog(
            archivedDocument.Id,
            "Archived",
            Serialize(previousView),
            Serialize(MapDocumentDetails(archivedDocument)),
            $"Archived warehouse document '{archivedDocument.Number}'."), cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return MapDocumentDetails(archivedDocument);
    }

    public async Task DeleteDocumentAsync(int documentId, RowVersionRequest request, CancellationToken cancellationToken = default)
    {
        if (request.RowVersion.Length == 0)
        {
            throw new ArgumentException("RowVersion is required when deleting a warehouse document.", nameof(request));
        }

        var document = await repository.GetDocumentForUpdateAsync(documentId, cancellationToken);
        if (document is null)
        {
            return;
        }

        repository.SetOriginalRowVersion(document, request.RowVersion);
        var previousView = MapDocumentDetails(document);
        repository.RemoveDocument(document);
        await repository.AddAuditLogAsync(CreateAuditLog(
            documentId,
            "Deleted",
            Serialize(previousView),
            "{}",
            $"Deleted warehouse document '{document.Number}'."), cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
    }

    public async Task<ProductView> CreateProductAsync(SaveWarehouseProductRequest request, CancellationToken cancellationToken = default)
    {
        ValidateProductRequest(request);

        var product = new Product(
            NormalizeRequired(request.Code, nameof(request.Code)),
            NormalizeRequired(request.Name, nameof(request.Name)),
            request.ProductCategoryId,
            request.UnitOfMeasureId,
            NormalizeOptional(request.Sku),
            request.MinimumStockLevel,
            request.IsActive);

        await repository.AddProductAsync(product, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        var createdProduct = await repository.GetProductAsync(product.Id, cancellationToken)
            ?? throw new InvalidOperationException("Created product could not be reloaded.");
        await repository.AddAuditLogAsync(CreateAuditLog(
            createdProduct.Id,
            "CreatedProduct",
            "{}",
            Serialize(MapProduct(createdProduct)),
            $"Created warehouse product '{createdProduct.Code}'."), cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return MapProduct(createdProduct);
    }

    public async Task<ProductView?> UpdateProductAsync(int productId, SaveWarehouseProductRequest request, CancellationToken cancellationToken = default)
    {
        if (request.RowVersion.Length == 0)
        {
            throw new ArgumentException("RowVersion is required when updating a product.", nameof(request));
        }

        ValidateProductRequest(request);

        var product = await repository.GetProductForUpdateAsync(productId, cancellationToken);
        if (product is null)
        {
            return null;
        }

        repository.SetOriginalRowVersion(product, request.RowVersion);
        var previousView = MapProduct(product);
        product.Update(
            request.Code.Trim(),
            request.Name.Trim(),
            request.ProductCategoryId,
            request.UnitOfMeasureId,
            NormalizeOptional(request.Sku),
            request.MinimumStockLevel,
            request.IsActive);

        await repository.SaveChangesAsync(cancellationToken);
        var updatedProduct = await repository.GetProductAsync(product.Id, cancellationToken)
            ?? throw new InvalidOperationException("Updated warehouse product could not be reloaded.");
        await repository.AddAuditLogAsync(CreateAuditLog(
            updatedProduct.Id,
            "UpdatedProduct",
            Serialize(previousView),
            Serialize(MapProduct(updatedProduct)),
            $"Updated warehouse product '{updatedProduct.Code}'."), cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return MapProduct(updatedProduct);
    }

    public async Task DeleteProductAsync(int productId, RowVersionRequest request, CancellationToken cancellationToken = default)
    {
        if (request.RowVersion.Length == 0)
        {
            throw new ArgumentException("RowVersion is required when deleting a product.", nameof(request));
        }

        var product = await repository.GetProductForUpdateAsync(productId, cancellationToken);
        if (product is null)
        {
            return;
        }

        repository.SetOriginalRowVersion(product, request.RowVersion);
        var previousView = MapProduct(product);
        repository.RemoveProduct(product);
        await repository.AddAuditLogAsync(CreateAuditLog(
            productId,
            "DeletedProduct",
            Serialize(previousView),
            "{}",
            $"Deleted warehouse product '{product.Code}'."), cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
    }

    public IReadOnlyList<WarehouseDocumentTypeOptionView> GetDocumentTypes()
    {
        return
        [
            new(nameof(WarehouseDocumentType.PZ), "PZ — przyjęcie zewnętrzne", false, true, false),
            new(nameof(WarehouseDocumentType.WZ), "WZ — wydanie zewnętrzne", true, false, false),
            new(nameof(WarehouseDocumentType.MM), "MM — przesunięcie międzymagazynowe", true, true, false),
            new(nameof(WarehouseDocumentType.RW), "RW — rozchód wewnętrzny", true, false, false),
            new(nameof(WarehouseDocumentType.PW), "PW — przyjęcie wewnętrzne", false, true, false),
            new(nameof(WarehouseDocumentType.INW), "INW — inwentaryzacja", false, true, true)
        ];
    }

    private StockItem CreateStockItem(StockMovement movement)
    {
        var stockItem = new StockItem(movement.WarehouseId, movement.WarehouseLocationId, movement.ProductId);
        repository.AddStockItem(stockItem);
        return stockItem;
    }

    private async Task<IReadOnlyList<StockMovement>> BuildMovementsAsync(
        WarehouseDocument document,
        WarehouseDocumentPosition position,
        DateTime movementDateUtc,
        CancellationToken cancellationToken)
    {
        return document.Type switch
        {
            WarehouseDocumentType.PZ => [CreateIncomingMovement(document, position, document.TargetWarehouseId!.Value, position.TargetLocationId ?? document.TargetLocationId, movementDateUtc)],
            WarehouseDocumentType.PW => [CreateIncomingMovement(document, position, document.TargetWarehouseId!.Value, position.TargetLocationId ?? document.TargetLocationId, movementDateUtc)],
            WarehouseDocumentType.WZ => [CreateOutgoingMovement(document, position, document.SourceWarehouseId!.Value, position.SourceLocationId ?? document.SourceLocationId, movementDateUtc)],
            WarehouseDocumentType.RW => [CreateOutgoingMovement(document, position, document.SourceWarehouseId!.Value, position.SourceLocationId ?? document.SourceLocationId, movementDateUtc)],
            WarehouseDocumentType.MM =>
            [
                CreateOutgoingMovement(document, position, document.SourceWarehouseId!.Value, position.SourceLocationId ?? document.SourceLocationId, movementDateUtc),
                CreateIncomingMovement(document, position, document.TargetWarehouseId!.Value, position.TargetLocationId ?? document.TargetLocationId, movementDateUtc)
            ],
            WarehouseDocumentType.INW => [await CreateInventoryAdjustmentMovementAsync(document, position, movementDateUtc, cancellationToken)],
            _ => throw new InvalidOperationException($"Unsupported warehouse document type '{document.Type}'.")
        };
    }

    private async Task<StockMovement> CreateInventoryAdjustmentMovementAsync(
        WarehouseDocument document,
        WarehouseDocumentPosition position,
        DateTime movementDateUtc,
        CancellationToken cancellationToken)
    {
        var warehouseId = document.TargetWarehouseId!.Value;
        var locationId = position.TargetLocationId ?? document.TargetLocationId;
        var currentStock = await repository.GetStockItemForUpdateAsync(warehouseId, locationId, position.ProductId, cancellationToken);
        var currentQuantity = currentStock?.QuantityOnHand ?? 0m;
        var delta = position.Quantity - currentQuantity;
        return new StockMovement(document.Id, position.Id, position.ProductId, warehouseId, locationId, delta, position.UnitPrice, movementDateUtc);
    }

    private static StockMovement CreateIncomingMovement(
        WarehouseDocument document,
        WarehouseDocumentPosition position,
        int warehouseId,
        int? locationId,
        DateTime movementDateUtc)
        => new(document.Id, position.Id, position.ProductId, warehouseId, locationId, position.Quantity, position.UnitPrice, movementDateUtc);

    private static StockMovement CreateOutgoingMovement(
        WarehouseDocument document,
        WarehouseDocumentPosition position,
        int warehouseId,
        int? locationId,
        DateTime movementDateUtc)
        => new(document.Id, position.Id, position.ProductId, warehouseId, locationId, -position.Quantity, position.UnitPrice, movementDateUtc);

    private AuditLog CreateAuditLog(int documentId, string actionName, string oldValues, string newValues, string summary)
        => new("WarehouseDocument", documentId, actionName, currentUserAccessor.UserId, oldValues, newValues, summary)
        {
            ChangedAtUtc = DateTime.UtcNow
        };

    private static IReadOnlyList<WarehouseDocumentPosition> MapPositions(IReadOnlyList<SaveWarehouseDocumentPositionRequest> positions)
    {
        if (positions.Count == 0)
        {
            throw new ArgumentException("Warehouse document must contain at least one position.", nameof(positions));
        }

        return positions.Select(item =>
            new WarehouseDocumentPosition(
                item.ProductId,
                item.Quantity,
                item.UnitPrice,
                item.SourceLocationId,
                item.TargetLocationId,
                NormalizeOptional(item.Notes))).ToArray();
    }

    private static WarehouseDocumentType ParseDocumentType(string value)
    {
        if (!Enum.TryParse<WarehouseDocumentType>(value, ignoreCase: true, out var parsed))
        {
            throw new ArgumentException($"Unknown warehouse document type '{value}'.", nameof(value));
        }

        return parsed;
    }

    private static void ValidateDocumentRequest(WarehouseDocumentType type, SaveWarehouseDocumentRequest request)
    {
        _ = NormalizeOptional(request.ExternalReference);
        _ = NormalizeOptional(request.Notes);

        if (request.Positions.Count == 0)
        {
            throw new ArgumentException("Warehouse document must contain at least one position.", nameof(request.Positions));
        }

        if ((type is WarehouseDocumentType.WZ or WarehouseDocumentType.RW or WarehouseDocumentType.MM) && request.SourceWarehouseId is null)
        {
            throw new ArgumentException("Source warehouse is required for this document type.", nameof(request.SourceWarehouseId));
        }

        if ((type is WarehouseDocumentType.PZ or WarehouseDocumentType.PW or WarehouseDocumentType.MM or WarehouseDocumentType.INW) && request.TargetWarehouseId is null)
        {
            throw new ArgumentException("Target warehouse is required for this document type.", nameof(request.TargetWarehouseId));
        }
    }

    private static void ValidateProductRequest(SaveWarehouseProductRequest request)
    {
        _ = NormalizeRequired(request.Code, nameof(request.Code));
        _ = NormalizeRequired(request.Name, nameof(request.Name));

        if (request.ProductCategoryId <= 0)
        {
            throw new ArgumentException("Product category is required.", nameof(request.ProductCategoryId));
        }

        if (request.UnitOfMeasureId <= 0)
        {
            throw new ArgumentException("Unit of measure is required.", nameof(request.UnitOfMeasureId));
        }

        if (request.MinimumStockLevel < 0m)
        {
            throw new ArgumentException("Minimum stock level cannot be negative.", nameof(request.MinimumStockLevel));
        }
    }

    private static string GetSequenceKey(WarehouseDocumentType type) => $"WH_{type}";

    private static ProductView MapProduct(Product product)
        => new(
            product.Id,
            product.Code,
            product.Name,
            product.Sku,
            product.IsActive,
            product.MinimumStockLevel,
            product.ProductCategoryId,
            product.ProductCategory.Code,
            product.ProductCategory.Name,
            product.UnitOfMeasureId,
            product.UnitOfMeasure.Code,
            product.UnitOfMeasure.Symbol,
            product.ModifiedAt ?? product.CreatedAt,
            product.RowVersion);

    private static StockItemView MapStockItem(StockItem item)
        => new(
            item.Id,
            item.WarehouseId,
            item.Warehouse.Code,
            item.Warehouse.Name,
            item.WarehouseLocationId,
            item.WarehouseLocation?.Code,
            item.WarehouseLocation?.Name,
            item.ProductId,
            item.Product.Code,
            item.Product.Name,
            item.Product.UnitOfMeasure.Symbol,
            item.QuantityOnHand,
            item.LastMovementAtUtc);

    private static WarehouseDocumentListItemView MapDocumentListItem(WarehouseDocument document)
        => new(
            document.Id,
            document.Number,
            document.Type.ToString(),
            document.Status.ToString(),
            document.DocumentDate,
            document.ContractorId,
            document.Contractor?.Name,
            document.SourceWarehouseId,
            document.SourceWarehouse?.Code,
            document.TargetWarehouseId,
            document.TargetWarehouse?.Code,
            document.Positions.Count,
            document.RowVersion);

    private static WarehouseDocumentDetailsView MapDocumentDetails(WarehouseDocument document)
        => new(
            document.Id,
            document.Number,
            document.Type.ToString(),
            document.Status.ToString(),
            document.DocumentDate,
            document.ContractorId,
            document.Contractor?.Name,
            document.SourceWarehouseId,
            document.SourceWarehouse?.Code,
            document.SourceLocationId,
            document.SourceLocation?.Code,
            document.TargetWarehouseId,
            document.TargetWarehouse?.Code,
            document.TargetLocationId,
            document.TargetLocation?.Code,
            document.ExternalReference,
            document.Notes,
            document.PostedAtUtc,
            document.PostedByUserId,
            document.Positions
                .OrderBy(item => item.Id)
                .Select(item => new WarehouseDocumentPositionView(
                    item.Id,
                    item.ProductId,
                    item.Product.Code,
                    item.Product.Name,
                    item.Quantity,
                    item.UnitPrice,
                    item.SourceLocationId,
                    item.SourceLocation?.Code,
                    item.SourceLocation?.Name,
                    item.TargetLocationId,
                    item.TargetLocation?.Code,
                    item.TargetLocation?.Name,
                    item.Notes))
                .ToArray(),
            document.StockMovements
                .OrderBy(item => item.MovementDateUtc)
                .ThenBy(item => item.Id)
                .Select(item => new StockMovementView(
                    item.Id,
                    item.ProductId,
                    item.Product.Code,
                    item.Product.Name,
                    item.WarehouseId,
                    item.Warehouse.Code,
                    item.Warehouse.Name,
                    item.WarehouseLocationId,
                    item.WarehouseLocation?.Code,
                    item.WarehouseLocation?.Name,
                    item.QuantityDelta,
                    item.UnitPrice,
                    item.MovementDateUtc))
                .ToArray(),
            document.RowVersion);

    private static string NormalizeRequired(string? value, string fieldName)
    {
        if (value is null)
        {
            throw new ArgumentException($"{fieldName} is required.", fieldName);
        }

        var normalized = value.Trim();
        if (normalized.Length == 0)
        {
            throw new ArgumentException($"{fieldName} is required.", fieldName);
        }

        return normalized;
    }

    private static string? NormalizeOptional(string? value)
    {
        if (value is null)
        {
            return null;
        }

        var normalized = value.Trim();
        return normalized.Length == 0 ? null : normalized;
    }

    private static string Serialize<T>(T value) => JsonSerializer.Serialize(value, SerializerOptions);
}
