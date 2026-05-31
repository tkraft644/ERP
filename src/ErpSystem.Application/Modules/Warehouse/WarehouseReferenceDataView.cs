namespace ErpSystem.Application.Modules.Warehouse;

public sealed record WarehouseReferenceDataView(
    IReadOnlyList<WarehouseOptionView> Warehouses,
    IReadOnlyList<WarehouseLocationView> Locations,
    IReadOnlyList<ProductCategoryView> Categories,
    IReadOnlyList<UnitOfMeasureView> UnitsOfMeasure,
    IReadOnlyList<ProductView> Products,
    IReadOnlyList<WarehouseDocumentTypeOptionView> DocumentTypes);
