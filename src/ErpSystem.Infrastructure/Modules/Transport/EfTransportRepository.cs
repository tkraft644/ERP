using ErpSystem.Application.Common;
using ErpSystem.Application.Modules.Transport;
using ErpSystem.Domain.Modules.Contractors;
using ErpSystem.Domain.Modules.HR;
using ErpSystem.Domain.Modules.System.Auditing;
using ErpSystem.Domain.Modules.Transport;
using ErpSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ErpSystem.Infrastructure.Modules.Transport;

public sealed class EfTransportRepository : ITransportRepository
{
    private readonly ErpSystemDbContext dbContext;

    public EfTransportRepository(ErpSystemDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Contractor>> GetPartnersAsync(CancellationToken cancellationToken = default)
        => await dbContext.Contractors
            .AsNoTracking()
            .Where(item => item.IsActive)
            .OrderBy(item => item.Name)
            .ToArrayAsync(cancellationToken);

    public async Task<IReadOnlyList<Carrier>> GetCarriersAsync(CancellationToken cancellationToken = default)
        => await dbContext.Carriers
            .AsNoTracking()
            .Where(item => item.IsActive)
            .Include(item => item.Contractor)
            .OrderBy(item => item.Contractor.Name)
            .ToArrayAsync(cancellationToken);

    public async Task<IReadOnlyList<Vehicle>> GetVehiclesAsync(CancellationToken cancellationToken = default)
        => await dbContext.Vehicles
            .AsNoTracking()
            .Where(item => item.IsActive)
            .OrderBy(item => item.RegistrationNumber)
            .ToArrayAsync(cancellationToken);

    public async Task<IReadOnlyList<Trailer>> GetTrailersAsync(CancellationToken cancellationToken = default)
        => await dbContext.Trailers
            .AsNoTracking()
            .Where(item => item.IsActive)
            .OrderBy(item => item.RegistrationNumber)
            .ToArrayAsync(cancellationToken);

    public async Task<IReadOnlyList<DriverProfile>> GetDriversAsync(CancellationToken cancellationToken = default)
        => await dbContext.DriverProfiles
            .AsNoTracking()
            .Include(item => item.Employee)
            .Where(item => item.Employee.IsActive)
            .OrderBy(item => item.Employee.LastName)
            .ThenBy(item => item.Employee.FirstName)
            .ToArrayAsync(cancellationToken);

    public async Task<IReadOnlyList<TransportOrder>> GetOrdersAsync(CancellationToken cancellationToken = default)
        => await BuildOrderQuery(asNoTracking: true)
            .OrderByDescending(item => item.OrderDate)
            .ThenByDescending(item => item.Id)
            .ToArrayAsync(cancellationToken);

    public Task<TransportOrder?> GetOrderAsync(int orderId, CancellationToken cancellationToken = default)
        => BuildOrderQuery(asNoTracking: true).FirstOrDefaultAsync(item => item.Id == orderId, cancellationToken);

    public Task<TransportOrder?> GetOrderForUpdateAsync(int orderId, CancellationToken cancellationToken = default)
        => BuildOrderQuery(asNoTracking: false).FirstOrDefaultAsync(item => item.Id == orderId, cancellationToken);

    public Task AddOrderAsync(TransportOrder order, CancellationToken cancellationToken = default)
        => dbContext.TransportOrders.AddAsync(order, cancellationToken).AsTask();

    public Task AddStatusHistoryAsync(TransportStatusHistory historyEntry, CancellationToken cancellationToken = default)
        => dbContext.TransportStatusHistoryEntries.AddAsync(historyEntry, cancellationToken).AsTask();

    public Task AddAuditLogAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
        => dbContext.AuditLogs.AddAsync(auditLog, cancellationToken).AsTask();

    public Task<string> GenerateDocumentNumberAsync(string key, DateTime utcNow, CancellationToken cancellationToken = default)
        => GenerateDocumentNumberInternalAsync(key, utcNow, cancellationToken);

    public void SetOriginalRowVersion(TransportOrder order, byte[] rowVersion)
    {
        dbContext.Entry(order).Property(item => item.RowVersion).OriginalValue = rowVersion;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            throw new ConcurrencyConflictException("The transport order was modified by another user. Refresh the data and try again.", exception);
        }
    }

    private IQueryable<TransportOrder> BuildOrderQuery(bool asNoTracking)
    {
        var query = dbContext.TransportOrders
            .Include(item => item.Carrier)
                .ThenInclude(item => item!.Contractor)
            .Include(item => item.Vehicle)
            .Include(item => item.Trailer)
            .Include(item => item.Driver)
                .ThenInclude(item => item!.Employee)
            .Include(item => item.Route)
            .Include(item => item.Stops)
                .ThenInclude(item => item.Contractor)
            .Include(item => item.Documents)
            .Include(item => item.Costs)
            .Include(item => item.StatusHistory)
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
