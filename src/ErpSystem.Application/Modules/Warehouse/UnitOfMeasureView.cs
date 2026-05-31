namespace ErpSystem.Application.Modules.Warehouse;

public sealed record UnitOfMeasureView(
    int Id,
    string Code,
    string Name,
    string Symbol,
    int DecimalPrecision);
