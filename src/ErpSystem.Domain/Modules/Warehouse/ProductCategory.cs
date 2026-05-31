namespace ErpSystem.Domain.Modules.Warehouse;

public sealed class ProductCategory : Common.AuditableEntity
{
    private ProductCategory()
    {
    }

    public ProductCategory(string code, string name, string? description)
    {
        Code = code;
        Name = name;
        Description = description;
    }

    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
}
