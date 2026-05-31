using ErpSystem.Domain.Modules.Transport;

namespace ErpSystem.Infrastructure.Persistence.Seed;

internal static class TransportSeedData
{
    private static readonly DateTime SeedTimestamp = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static Carrier[] Carriers =>
    [
        Create(new Carrier(3, "CAR-TRANS-POL", isPreferred: true), 1),
        Create(new Carrier(5, "CAR-INTER-EU"), 2)
    ];

    public static Vehicle[] Vehicles =>
    [
        Create(new Vehicle("WPR 1001A", "DAF", "XF 480", 18.0m, 1), 1),
        Create(new Vehicle("WPR 2002B", "Volvo", "FH 460", 18.0m, 2), 2)
    ];

    public static Trailer[] Trailers =>
    [
        Create(new Trailer("WPR 3003C", "Firanka", 24.0m, 1), 1),
        Create(new Trailer("WPR 4004D", "Chłodnia", 22.0m, 2), 2)
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
