using ErpSystem.Domain.Modules.Contractors;
using ErpSystem.Domain.Modules.Finance;
using ErpSystem.Domain.Modules.HR;
using ErpSystem.Domain.Modules.System.Auditing;
using ErpSystem.Domain.Modules.System.Dictionaries;
using ErpSystem.Domain.Modules.Transport;
using WarehouseEntity = ErpSystem.Domain.Modules.Warehouse.Warehouse;

namespace ErpSystem.Application.Modules.Finance;

public interface IFinanceRepository
{
    Task<IReadOnlyList<Contractor>> GetContractorsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TransportOrder>> GetTransportOrdersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Vehicle>> GetVehiclesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Employee>> GetEmployeesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WarehouseEntity>> GetWarehousesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Department>> GetDepartmentsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DictionaryItem>> GetCurrenciesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CostDocument>> GetCostDocumentsAsync(CancellationToken cancellationToken = default);
    Task<CostDocument?> GetCostDocumentAsync(int costDocumentId, CancellationToken cancellationToken = default);
    Task<CostDocument?> GetCostDocumentForUpdateAsync(int costDocumentId, CancellationToken cancellationToken = default);
    Task AddCostDocumentAsync(CostDocument costDocument, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Invoice>> GetInvoicesAsync(CancellationToken cancellationToken = default);
    Task<Invoice?> GetInvoiceAsync(int invoiceId, CancellationToken cancellationToken = default);
    Task<Invoice?> GetInvoiceForUpdateAsync(int invoiceId, CancellationToken cancellationToken = default);
    Task AddInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Payment>> GetPaymentsAsync(CancellationToken cancellationToken = default);
    Task<Payment?> GetPaymentForSettlementAsync(int paymentId, CancellationToken cancellationToken = default);
    Task AddPaymentAsync(Payment payment, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CurrencyRate>> GetCurrencyRatesAsync(CancellationToken cancellationToken = default);
    Task<CurrencyRate?> GetCurrencyRateForUpdateAsync(int currencyRateId, CancellationToken cancellationToken = default);
    Task AddCurrencyRateAsync(CurrencyRate currencyRate, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Settlement>> GetSettlementsAsync(CancellationToken cancellationToken = default);
    Task AddSettlementAsync(Settlement settlement, CancellationToken cancellationToken = default);

    Task AddAuditLogAsync(AuditLog auditLog, CancellationToken cancellationToken = default);
    Task<string> GenerateDocumentNumberAsync(string key, DateTime utcNow, CancellationToken cancellationToken = default);
    void SetOriginalRowVersion(CostDocument costDocument, byte[] rowVersion);
    void SetOriginalRowVersion(Invoice invoice, byte[] rowVersion);
    void SetOriginalRowVersion(CurrencyRate currencyRate, byte[] rowVersion);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
