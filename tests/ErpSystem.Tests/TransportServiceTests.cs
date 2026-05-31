using ErpSystem.Application.Common;
using ErpSystem.Application.Modules.Transport;
using ErpSystem.Domain.Modules.Contractors;
using ErpSystem.Domain.Modules.HR;
using ErpSystem.Domain.Modules.System.Auditing;
using ErpSystem.Domain.Modules.Transport;

namespace ErpSystem.Tests;

public class TransportServiceTests
{
    [Fact]
    public async Task CreateOrderAsync_ShouldRequireExchangeRateForInternationalOrder()
    {
        var service = new TransportService(new TestTransportRepository(), new TestCurrentUserAccessor(7));

        var request = new SaveTransportOrderRequest(
            "International",
            new DateTime(2026, 5, 14, 8, 0, 0, DateTimeKind.Utc),
            null,
            null,
            null,
            null,
            "PL",
            "DE",
            "FCA",
            "EUR",
            null,
            true,
            true,
            null,
            null,
            "REF-100",
            null,
            new SaveTransportRouteRequest(650m, 4200m, DateTime.UtcNow, DateTime.UtcNow.AddHours(12), "PL -> DE"),
            CreateStopRequests(),
            [],
            []);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.CreateOrderAsync(request));

        Assert.Equal("ExchangeRate", exception.ParamName);
    }

    [Fact]
    public async Task ChangeStatusAsync_ShouldRejectInvalidTransition()
    {
        var repository = new TestTransportRepository
        {
            OrderForUpdate = CreateDomesticOrder()
        };
        var service = new TransportService(repository, new TestCurrentUserAccessor(7));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.ChangeStatusAsync(
                repository.OrderForUpdate!.Id,
                new ChangeTransportOrderStatusRequest("Delivered", "Skipping planning", [1])));

        Assert.Contains("not allowed", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    private static TransportOrder CreateDomesticOrder()
    {
        var order = new TransportOrder(
            "TR/202605/00042",
            TransportOrderType.Domestic,
            new DateTime(2026, 5, 14, 8, 0, 0, DateTimeKind.Utc),
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            false,
            false,
            "Mazowieckie",
            "FTL",
            "REF-42",
            null)
        {
            Id = 42,
            RowVersion = [1]
        };

        order.ReplaceRoute(new TransportOrderRoute(320m, 1800m, DateTime.UtcNow, DateTime.UtcNow.AddHours(6), "Warszawa -> Poznań"));
        order.ReplaceStops(CreateStops());
        return order;
    }

    private static IReadOnlyList<TransportStop> CreateStops()
        =>
        [
            new TransportStop(1, TransportStopType.Loading, null, "Magazyn Warszawa", "PL", "Warszawa", "ul. Startowa 1", DateTime.UtcNow, null, null),
            new TransportStop(2, TransportStopType.Unloading, null, "Centrum Poznań", "PL", "Poznań", "ul. Końcowa 9", DateTime.UtcNow.AddHours(5), null, null)
        ];

    private static IReadOnlyList<SaveTransportStopRequest> CreateStopRequests()
        =>
        [
            new SaveTransportStopRequest(1, "Loading", null, "Magazyn Warszawa", "PL", "Warszawa", "ul. Startowa 1", DateTime.UtcNow, null, null),
            new SaveTransportStopRequest(2, "Unloading", null, "Centrum Poznań", "PL", "Poznań", "ul. Końcowa 9", DateTime.UtcNow.AddHours(5), null, null)
        ];

    private sealed class TestCurrentUserAccessor : ICurrentUserAccessor
    {
        public TestCurrentUserAccessor(int? userId)
        {
            UserId = userId;
        }

        public int? UserId { get; }
    }

    private sealed class TestTransportRepository : ITransportRepository
    {
        public TransportOrder? OrderForUpdate { get; set; }

        public Task<IReadOnlyList<Contractor>> GetPartnersAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Contractor>>([]);

        public Task<IReadOnlyList<Carrier>> GetCarriersAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Carrier>>([]);

        public Task<IReadOnlyList<Vehicle>> GetVehiclesAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Vehicle>>([]);

        public Task<IReadOnlyList<Trailer>> GetTrailersAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Trailer>>([]);

        public Task<IReadOnlyList<DriverProfile>> GetDriversAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<DriverProfile>>([]);

        public Task<IReadOnlyList<TransportOrder>> GetOrdersAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<TransportOrder>>([]);

        public Task<TransportOrder?> GetOrderAsync(int orderId, CancellationToken cancellationToken = default)
            => Task.FromResult<TransportOrder?>(OrderForUpdate);

        public Task<TransportOrder?> GetOrderForUpdateAsync(int orderId, CancellationToken cancellationToken = default)
            => Task.FromResult(OrderForUpdate);

        public Task AddOrderAsync(TransportOrder order, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task AddStatusHistoryAsync(TransportStatusHistory historyEntry, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task AddAuditLogAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<string> GenerateDocumentNumberAsync(string key, DateTime utcNow, CancellationToken cancellationToken = default)
            => Task.FromResult("TR/202605/00001");

        public void SetOriginalRowVersion(TransportOrder order, byte[] rowVersion)
        {
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }
}
