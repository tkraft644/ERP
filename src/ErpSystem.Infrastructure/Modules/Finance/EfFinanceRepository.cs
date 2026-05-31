using ErpSystem.Application.Common;
using ErpSystem.Application.Modules.Finance;
using ErpSystem.Domain.Modules.Contractors;
using ErpSystem.Domain.Modules.Finance;
using ErpSystem.Domain.Modules.HR;
using ErpSystem.Domain.Modules.System.Auditing;
using ErpSystem.Domain.Modules.System.Dictionaries;
using ErpSystem.Domain.Modules.Transport;
using ErpSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using WarehouseEntity = ErpSystem.Domain.Modules.Warehouse.Warehouse;

namespace ErpSystem.Infrastructure.Modules.Finance;

public sealed class EfFinanceRepository : IFinanceRepository
{
    private readonly ErpSystemDbContext dbContext;

    public EfFinanceRepository(ErpSystemDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Contractor>> GetContractorsAsync(CancellationToken cancellationToken = default)
        => await dbContext.Contractors.AsNoTracking().OrderBy(item => item.Name).ToArrayAsync(cancellationToken);

    public async Task<IReadOnlyList<TransportOrder>> GetTransportOrdersAsync(CancellationToken cancellationToken = default)
        => await dbContext.TransportOrders.AsNoTracking().OrderByDescending(item => item.OrderDate).ThenByDescending(item => item.Id).ToArrayAsync(cancellationToken);

    public async Task<IReadOnlyList<Vehicle>> GetVehiclesAsync(CancellationToken cancellationToken = default)
        => await dbContext.Vehicles.AsNoTracking().OrderBy(item => item.RegistrationNumber).ToArrayAsync(cancellationToken);

    public async Task<IReadOnlyList<Employee>> GetEmployeesAsync(CancellationToken cancellationToken = default)
        => await dbContext.Employees.AsNoTracking().OrderBy(item => item.LastName).ThenBy(item => item.FirstName).ToArrayAsync(cancellationToken);

    public async Task<IReadOnlyList<WarehouseEntity>> GetWarehousesAsync(CancellationToken cancellationToken = default)
        => await dbContext.Warehouses.AsNoTracking().OrderBy(item => item.Name).ToArrayAsync(cancellationToken);

    public async Task<IReadOnlyList<Department>> GetDepartmentsAsync(CancellationToken cancellationToken = default)
        => await dbContext.Departments.AsNoTracking().OrderBy(item => item.Name).ToArrayAsync(cancellationToken);

    public async Task<IReadOnlyList<DictionaryItem>> GetCurrenciesAsync(CancellationToken cancellationToken = default)
        => await dbContext.DictionaryItems
            .AsNoTracking()
            .Where(item => item.DictionaryName == "Currencies")
            .OrderBy(item => item.SortOrder)
            .ThenBy(item => item.Name)
            .ToArrayAsync(cancellationToken);

    public async Task<IReadOnlyList<CostDocument>> GetCostDocumentsAsync(CancellationToken cancellationToken = default)
        => await BuildCostDocumentQuery(asNoTracking: true)
            .OrderByDescending(item => item.DocumentDate)
            .ThenByDescending(item => item.Id)
            .ToArrayAsync(cancellationToken);

    public Task<CostDocument?> GetCostDocumentAsync(int costDocumentId, CancellationToken cancellationToken = default)
        => BuildCostDocumentQuery(asNoTracking: true).FirstOrDefaultAsync(item => item.Id == costDocumentId, cancellationToken);

    public Task<CostDocument?> GetCostDocumentForUpdateAsync(int costDocumentId, CancellationToken cancellationToken = default)
        => BuildCostDocumentQuery(asNoTracking: false).FirstOrDefaultAsync(item => item.Id == costDocumentId, cancellationToken);

    public Task AddCostDocumentAsync(CostDocument costDocument, CancellationToken cancellationToken = default)
        => dbContext.CostDocuments.AddAsync(costDocument, cancellationToken).AsTask();

    public async Task<IReadOnlyList<Invoice>> GetInvoicesAsync(CancellationToken cancellationToken = default)
        => await BuildInvoiceQuery(asNoTracking: true)
            .OrderByDescending(item => item.InvoiceDate)
            .ThenByDescending(item => item.Id)
            .ToArrayAsync(cancellationToken);

    public Task<Invoice?> GetInvoiceAsync(int invoiceId, CancellationToken cancellationToken = default)
        => BuildInvoiceQuery(asNoTracking: true).FirstOrDefaultAsync(item => item.Id == invoiceId, cancellationToken);

    public Task<Invoice?> GetInvoiceForUpdateAsync(int invoiceId, CancellationToken cancellationToken = default)
        => BuildInvoiceQuery(asNoTracking: false).FirstOrDefaultAsync(item => item.Id == invoiceId, cancellationToken);

    public Task AddInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default)
        => dbContext.Invoices.AddAsync(invoice, cancellationToken).AsTask();

    public async Task<IReadOnlyList<Payment>> GetPaymentsAsync(CancellationToken cancellationToken = default)
        => await dbContext.Payments
            .AsNoTracking()
            .Include(item => item.Contractor)
            .Include(item => item.Settlements)
            .OrderByDescending(item => item.PaymentDate)
            .ThenByDescending(item => item.Id)
            .ToArrayAsync(cancellationToken);

    public Task<Payment?> GetPaymentForSettlementAsync(int paymentId, CancellationToken cancellationToken = default)
        => dbContext.Payments
            .Include(item => item.Contractor)
            .Include(item => item.Settlements)
            .FirstOrDefaultAsync(item => item.Id == paymentId, cancellationToken);

    public Task AddPaymentAsync(Payment payment, CancellationToken cancellationToken = default)
        => dbContext.Payments.AddAsync(payment, cancellationToken).AsTask();

    public async Task<IReadOnlyList<CurrencyRate>> GetCurrencyRatesAsync(CancellationToken cancellationToken = default)
        => await dbContext.CurrencyRates
            .AsNoTracking()
            .OrderByDescending(item => item.RateDate)
            .ThenBy(item => item.BaseCurrencyCode)
            .ThenBy(item => item.QuoteCurrencyCode)
            .ToArrayAsync(cancellationToken);

    public Task<CurrencyRate?> GetCurrencyRateForUpdateAsync(int currencyRateId, CancellationToken cancellationToken = default)
        => dbContext.CurrencyRates.FirstOrDefaultAsync(item => item.Id == currencyRateId, cancellationToken);

    public Task AddCurrencyRateAsync(CurrencyRate currencyRate, CancellationToken cancellationToken = default)
        => dbContext.CurrencyRates.AddAsync(currencyRate, cancellationToken).AsTask();

    public async Task<IReadOnlyList<Settlement>> GetSettlementsAsync(CancellationToken cancellationToken = default)
        => await dbContext.Settlements
            .AsNoTracking()
            .Include(item => item.Payment)
            .Include(item => item.Invoice)
            .Include(item => item.CostDocument)
            .OrderByDescending(item => item.SettledAtUtc)
            .ThenByDescending(item => item.Id)
            .ToArrayAsync(cancellationToken);

    public Task AddSettlementAsync(Settlement settlement, CancellationToken cancellationToken = default)
        => dbContext.Settlements.AddAsync(settlement, cancellationToken).AsTask();

    public Task AddAuditLogAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
        => dbContext.AuditLogs.AddAsync(auditLog, cancellationToken).AsTask();

    public Task<string> GenerateDocumentNumberAsync(string key, DateTime utcNow, CancellationToken cancellationToken = default)
        => GenerateDocumentNumberInternalAsync(key, utcNow, cancellationToken);

    public void SetOriginalRowVersion(CostDocument costDocument, byte[] rowVersion)
    {
        dbContext.Entry(costDocument).Property(item => item.RowVersion).OriginalValue = rowVersion;
    }

    public void SetOriginalRowVersion(Invoice invoice, byte[] rowVersion)
    {
        dbContext.Entry(invoice).Property(item => item.RowVersion).OriginalValue = rowVersion;
    }

    public void SetOriginalRowVersion(CurrencyRate currencyRate, byte[] rowVersion)
    {
        dbContext.Entry(currencyRate).Property(item => item.RowVersion).OriginalValue = rowVersion;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            throw new ConcurrencyConflictException("The finance data was modified by another user. Refresh the data and try again.", exception);
        }
    }

    private IQueryable<CostDocument> BuildCostDocumentQuery(bool asNoTracking)
    {
        var query = dbContext.CostDocuments
            .Include(item => item.Contractor)
            .Include(item => item.Positions)
                .ThenInclude(item => item.TransportOrder)
            .Include(item => item.Positions)
                .ThenInclude(item => item.Vehicle)
            .Include(item => item.Positions)
                .ThenInclude(item => item.Employee)
            .Include(item => item.Positions)
                .ThenInclude(item => item.Warehouse)
            .Include(item => item.Positions)
                .ThenInclude(item => item.Contractor)
            .Include(item => item.Positions)
                .ThenInclude(item => item.Department)
            .Include(item => item.Settlements)
            .AsQueryable();

        return asNoTracking ? query.AsNoTracking() : query;
    }

    private IQueryable<Invoice> BuildInvoiceQuery(bool asNoTracking)
    {
        var query = dbContext.Invoices
            .Include(item => item.Contractor)
            .Include(item => item.TransportOrder)
            .Include(item => item.Positions)
            .Include(item => item.Settlements)
            .AsQueryable();

        return asNoTracking ? query.AsNoTracking() : query;
    }

    private async Task<string> GenerateDocumentNumberInternalAsync(string key, DateTime utcNow, CancellationToken cancellationToken)
    {
        var sequence = await dbContext.DocumentNumberSequences.FirstOrDefaultAsync(item => item.Key == key, cancellationToken);
        if (sequence is null)
        {
            throw new InvalidOperationException($"Document sequence '{key}' was not found.");
        }

        return sequence.GenerateNextNumber(utcNow);
    }
}
