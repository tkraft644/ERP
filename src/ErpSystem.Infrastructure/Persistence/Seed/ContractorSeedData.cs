using ErpSystem.Domain.Modules.Contractors;

namespace ErpSystem.Infrastructure.Persistence.Seed;

internal static class ContractorSeedData
{
    private static readonly DateTime SeedTimestamp = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static Contractor[] Contractors =>
    [
        Create(new Contractor(
            "CTR-001",
            "Baltic Retail Sp. z o.o.",
            "Baltic Retail",
            "5253001001",
            ContractorType.Client | ContractorType.Receiver), 1),
        Create(new Contractor(
            "CTR-002",
            "Nordic Components GmbH",
            "Nordic Components",
            "DE123456789",
            ContractorType.Supplier | ContractorType.Sender), 2),
        Create(new Contractor(
            "CTR-003",
            "Trans-Pol Logistics Sp. z o.o.",
            "Trans-Pol",
            "5272003003",
            ContractorType.Carrier | ContractorType.Sender), 3),
        Create(new Contractor(
            "CTR-004",
            "Euro Customs Agency Sp. z o.o.",
            "Euro Customs",
            "5264004004",
            ContractorType.CustomsAgency | ContractorType.Service), 4),
        Create(new Contractor(
            "CTR-005",
            "Inter Cargo EU s.r.o.",
            "Inter Cargo EU",
            "CZ12345678",
            ContractorType.Carrier | ContractorType.Receiver), 5)
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
