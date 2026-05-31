using ErpSystem.Application.Common;
using ErpSystem.Application.Modules.Finance;
using ErpSystem.Domain.Modules.Contractors;
using ErpSystem.Domain.Modules.Finance;
using ErpSystem.Domain.Modules.HR;
using ErpSystem.Domain.Modules.System.Auditing;
using ErpSystem.Domain.Modules.System.Dictionaries;
using ErpSystem.Domain.Modules.Transport;
using WarehouseEntity = ErpSystem.Domain.Modules.Warehouse.Warehouse;

namespace ErpSystem.Tests;

public class FinanceServiceTests
{
    [Fact]
    public async Task CreateCostDocumentAsync_ShouldRequireAtLeastOnePosition()
    {
        var service = new FinanceService(new TestFinanceRepository(), new TestCurrentUserAccessor(5));

        var request = new SaveCostDocumentRequest(
            new DateTime(2026, 5, 14),
            new DateTime(2026, 5, 14),
            null,
            null,
            "PLN",
            null,
            null,
            null,
            []);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.CreateCostDocumentAsync(request));

        Assert.Equal("Positions", exception.ParamName);
    }

    [Fact]
    public async Task CreateSettlementAsync_ShouldRecalculateInvoiceStatus()
    {
        var repository = new TestFinanceRepository();
        repository.PaymentForSettlement = new Payment(
            new DateTime(2026, 5, 14),
            PaymentDirection.Incoming,
            1,
            "PLN",
            null,
            123m,
            "Transfer",
            "PAY-1",
            null)
        {
            Id = 7
        };
        repository.InvoiceForUpdate = CreateInvoice();

        var service = new FinanceService(repository, new TestCurrentUserAccessor(5));

        var settlement = await service.CreateSettlementAsync(new SaveSettlementRequest(
            repository.PaymentForSettlement.Id,
            repository.InvoiceForUpdate.Id,
            null,
            123m,
            "Rozliczenie faktury"));

        Assert.Equal(repository.InvoiceForUpdate.Id, settlement.InvoiceId);
        Assert.Equal(InvoiceStatus.Paid, repository.InvoiceForUpdate.Status);
        Assert.Single(repository.InvoiceForUpdate.Settlements);
        Assert.Single(repository.PaymentForSettlement.Settlements);
    }

    private static Invoice CreateInvoice()
    {
        var contractor = new Contractor("CTR-100", "Test Contractor", null, null, ContractorType.Client)
        {
            Id = 1
        };
        var invoice = new Invoice(
            "FV/202605/0001",
            new DateTime(2026, 5, 14),
            new DateTime(2026, 5, 14),
            new DateTime(2026, 5, 28),
            contractor.Id,
            null,
            "PLN",
            null,
            null,
            null)
        {
            Id = 11,
            RowVersion = [1]
        };

        invoice.ReplacePositions(
        [
            new InvoicePosition(1, "Transport krajowy", "Usługa transportowa", 1m, 100m, 23m, null)
        ]);
        invoice.RecalculateStatus(0m);
        typeof(Invoice).GetProperty(nameof(Invoice.Contractor))!.SetValue(invoice, contractor);

        return invoice;
    }

    private sealed class TestCurrentUserAccessor : ICurrentUserAccessor
    {
        public TestCurrentUserAccessor(int? userId)
        {
            UserId = userId;
        }

        public int? UserId { get; }
    }

    private sealed class TestFinanceRepository : IFinanceRepository
    {
        private readonly List<Settlement> settlements = [];

        public Payment? PaymentForSettlement { get; set; }
        public Invoice? InvoiceForUpdate { get; set; }
        public CostDocument? CostDocumentForUpdate { get; set; }

        public Task<IReadOnlyList<Contractor>> GetContractorsAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Contractor>>([]);

        public Task<IReadOnlyList<TransportOrder>> GetTransportOrdersAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<TransportOrder>>([]);

        public Task<IReadOnlyList<Vehicle>> GetVehiclesAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Vehicle>>([]);

        public Task<IReadOnlyList<Employee>> GetEmployeesAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Employee>>([]);

        public Task<IReadOnlyList<WarehouseEntity>> GetWarehousesAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<WarehouseEntity>>([]);

        public Task<IReadOnlyList<Department>> GetDepartmentsAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Department>>([]);

        public Task<IReadOnlyList<DictionaryItem>> GetCurrenciesAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<DictionaryItem>>([]);

        public Task<IReadOnlyList<CostDocument>> GetCostDocumentsAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<CostDocument>>([]);

        public Task<CostDocument?> GetCostDocumentAsync(int costDocumentId, CancellationToken cancellationToken = default)
            => Task.FromResult(CostDocumentForUpdate);

        public Task<CostDocument?> GetCostDocumentForUpdateAsync(int costDocumentId, CancellationToken cancellationToken = default)
            => Task.FromResult(CostDocumentForUpdate);

        public Task AddCostDocumentAsync(CostDocument costDocument, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<IReadOnlyList<Invoice>> GetInvoicesAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Invoice>>(InvoiceForUpdate is null ? [] : [InvoiceForUpdate]);

        public Task<Invoice?> GetInvoiceAsync(int invoiceId, CancellationToken cancellationToken = default)
            => Task.FromResult(InvoiceForUpdate);

        public Task<Invoice?> GetInvoiceForUpdateAsync(int invoiceId, CancellationToken cancellationToken = default)
            => Task.FromResult(InvoiceForUpdate);

        public Task AddInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<IReadOnlyList<Payment>> GetPaymentsAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Payment>>(PaymentForSettlement is null ? [] : [PaymentForSettlement]);

        public Task<Payment?> GetPaymentForSettlementAsync(int paymentId, CancellationToken cancellationToken = default)
            => Task.FromResult(PaymentForSettlement);

        public Task AddPaymentAsync(Payment payment, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<IReadOnlyList<CurrencyRate>> GetCurrencyRatesAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<CurrencyRate>>([]);

        public Task<CurrencyRate?> GetCurrencyRateForUpdateAsync(int currencyRateId, CancellationToken cancellationToken = default)
            => Task.FromResult<CurrencyRate?>(null);

        public Task AddCurrencyRateAsync(CurrencyRate currencyRate, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<IReadOnlyList<Settlement>> GetSettlementsAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Settlement>>(settlements);

        public Task AddSettlementAsync(Settlement settlement, CancellationToken cancellationToken = default)
        {
            settlement.Id = settlements.Count + 1;
            settlement.RowVersion = [1];

            if (PaymentForSettlement is not null)
            {
                typeof(Settlement).GetProperty(nameof(Settlement.Payment))!.SetValue(settlement, PaymentForSettlement);
                PaymentForSettlement.Settlements.Add(settlement);
            }

            if (InvoiceForUpdate is not null && settlement.InvoiceId == InvoiceForUpdate.Id)
            {
                typeof(Settlement).GetProperty(nameof(Settlement.Invoice))!.SetValue(settlement, InvoiceForUpdate);
                InvoiceForUpdate.Settlements.Add(settlement);
            }

            if (CostDocumentForUpdate is not null && settlement.CostDocumentId == CostDocumentForUpdate.Id)
            {
                typeof(Settlement).GetProperty(nameof(Settlement.CostDocument))!.SetValue(settlement, CostDocumentForUpdate);
                CostDocumentForUpdate.Settlements.Add(settlement);
            }

            settlements.Add(settlement);
            return Task.CompletedTask;
        }

        public Task AddAuditLogAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<string> GenerateDocumentNumberAsync(string key, DateTime utcNow, CancellationToken cancellationToken = default)
            => Task.FromResult($"{key}/0001");

        public void SetOriginalRowVersion(CostDocument costDocument, byte[] rowVersion)
        {
        }

        public void SetOriginalRowVersion(Invoice invoice, byte[] rowVersion)
        {
        }

        public void SetOriginalRowVersion(CurrencyRate currencyRate, byte[] rowVersion)
        {
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }
}
