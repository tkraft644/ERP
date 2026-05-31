namespace ErpSystem.Domain.Modules.Warehouse;

public sealed class UnitOfMeasure : Common.AuditableEntity
{
    private UnitOfMeasure()
    {
    }

    public UnitOfMeasure(string code, string name, string symbol, int decimalPrecision = 2)
    {
        Code = code;
        Name = name;
        Symbol = symbol;
        DecimalPrecision = decimalPrecision;
    }

    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Symbol { get; private set; } = string.Empty;
    public int DecimalPrecision { get; private set; }
}
