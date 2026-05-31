namespace ErpSystem.Domain.Modules.Warehouse;

public sealed class Product : Common.AuditableEntity
{
    private Product()
    {
    }

    public Product(
        string code,
        string name,
        int categoryId,
        int unitOfMeasureId,
        string? sku,
        decimal minimumStockLevel = 0m,
        bool isActive = true)
    {
        Code = code;
        Name = name;
        ProductCategoryId = categoryId;
        UnitOfMeasureId = unitOfMeasureId;
        Sku = sku;
        MinimumStockLevel = minimumStockLevel;
        IsActive = isActive;
    }

    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public int ProductCategoryId { get; private set; }
    public ProductCategory ProductCategory { get; private set; } = null!;
    public int UnitOfMeasureId { get; private set; }
    public UnitOfMeasure UnitOfMeasure { get; private set; } = null!;
    public string? Sku { get; private set; }
    public decimal MinimumStockLevel { get; private set; }
    public bool IsActive { get; private set; }

    public void Update(
        string code,
        string name,
        int categoryId,
        int unitOfMeasureId,
        string? sku,
        decimal minimumStockLevel,
        bool isActive)
    {
        Code = code;
        Name = name;
        ProductCategoryId = categoryId;
        UnitOfMeasureId = unitOfMeasureId;
        Sku = sku;
        MinimumStockLevel = minimumStockLevel;
        IsActive = isActive;
    }
}
