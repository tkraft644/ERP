using ErpSystem.Domain.Modules.Warehouse;

namespace ErpSystem.Infrastructure.Persistence.Seed;

internal static class WarehouseSeedData
{
    private static readonly DateTime SeedTimestamp = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static Warehouse[] Warehouses =>
    [
        Create(new Warehouse("MAIN", "Magazyn główny", "Główny magazyn operacyjny."), 1),
        Create(new Warehouse("TRANSIT", "Magazyn tranzytowy", "Bufor dla wydań i przyjęć."), 2)
    ];

    public static WarehouseLocation[] Locations =>
    [
        Create(new WarehouseLocation(1, "A-01", "Regał A-01"), 1),
        Create(new WarehouseLocation(1, "DOCK-IN", "Strefa przyjęć"), 2),
        Create(new WarehouseLocation(2, "DOCK-OUT", "Strefa wydań"), 3)
    ];

    public static ProductCategory[] Categories =>
    [
        Create(new ProductCategory("TRADE", "Towary handlowe", "Produkty przeznaczone do odsprzedaży."), 1),
        Create(new ProductCategory("MAT", "Materiały", "Materiały i komponenty operacyjne."), 2)
    ];

    public static UnitOfMeasure[] Units =>
    [
        Create(new UnitOfMeasure("PCS", "Sztuka", "szt.", 2), 1),
        Create(new UnitOfMeasure("PAL", "Paleta", "pal.", 3), 2)
    ];

    public static Product[] Products =>
    [
        Create(new Product("PRD-001", "Płyn do spryskiwaczy", 1, 1, "590000000001", 20m), 1),
        Create(new Product("PRD-002", "Pojemnik transportowy", 2, 1, "590000000002", 10m), 2),
        Create(new Product("PRD-003", "Folia stretch", 2, 2, "590000000003", 12m), 3)
    ];

    private static T Create<T>(T entity, int id) where T : ErpSystem.Domain.Common.AuditableEntity
    {
        entity.Id = id;
        entity.CreatedAt = SeedTimestamp;
        entity.CreatedByUserId = FoundationSeedData.AdminUserId;
        entity.IsDeleted = false;
        entity.RowVersion = [];
        return entity;
    }
}
