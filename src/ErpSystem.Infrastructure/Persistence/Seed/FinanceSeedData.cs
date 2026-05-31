using ErpSystem.Domain.Modules.Finance;

namespace ErpSystem.Infrastructure.Persistence.Seed;

internal static class FinanceSeedData
{
    private static readonly DateTime SeedTimestamp = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static CurrencyRate[] CurrencyRates =>
    [
        Create(new CurrencyRate(new DateTime(2026, 5, 14), "EUR", "PLN", 4.28m, "NBP"), 1),
        Create(new CurrencyRate(new DateTime(2026, 5, 14), "USD", "PLN", 3.91m, "NBP"), 2)
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
