namespace ErpSystem.Application.Modules.Warehouse;

public sealed record WarehouseDocumentTypeOptionView(
    string Key,
    string Label,
    bool RequiresSourceWarehouse,
    bool RequiresTargetWarehouse,
    bool CreatesStockAdjustment);
