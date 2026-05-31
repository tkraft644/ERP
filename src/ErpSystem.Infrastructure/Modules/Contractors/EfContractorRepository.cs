using ErpSystem.Application.Common;
using ErpSystem.Application.Modules.Contractors;
using ErpSystem.Domain.Modules.Contractors;
using ErpSystem.Domain.Modules.System.Auditing;
using ErpSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ErpSystem.Infrastructure.Modules.Contractors;

public sealed class EfContractorRepository : IContractorRepository
{
    private readonly ErpSystemDbContext dbContext;

    public EfContractorRepository(ErpSystemDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Contractor>> GetContractorsAsync(bool includeInactive, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Contractors
            .AsNoTracking()
            .Include(item => item.Addresses)
            .Include(item => item.Contacts)
            .AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(item => item.IsActive);
        }

        return await query
            .OrderBy(item => item.Name)
            .ThenBy(item => item.Code)
            .ToArrayAsync(cancellationToken);
    }

    public Task<Contractor?> GetContractorAsync(int contractorId, CancellationToken cancellationToken = default)
        => dbContext.Contractors
            .AsNoTracking()
            .Include(item => item.Addresses)
            .Include(item => item.Contacts)
            .Include(item => item.BankAccounts)
            .Include(item => item.Notes)
            .FirstOrDefaultAsync(item => item.Id == contractorId, cancellationToken);

    public Task<Contractor?> GetContractorForUpdateAsync(int contractorId, CancellationToken cancellationToken = default)
        => dbContext.Contractors
            .Include(item => item.Addresses)
            .Include(item => item.Contacts)
            .Include(item => item.BankAccounts)
            .Include(item => item.Notes)
            .FirstOrDefaultAsync(item => item.Id == contractorId, cancellationToken);

    public Task AddContractorAsync(Contractor contractor, CancellationToken cancellationToken = default)
        => dbContext.Contractors.AddAsync(contractor, cancellationToken).AsTask();

    public Task AddAuditLogAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
        => dbContext.AuditLogs.AddAsync(auditLog, cancellationToken).AsTask();

    public async Task<IReadOnlyList<AuditLog>> GetContractorHistoryAsync(int contractorId, CancellationToken cancellationToken = default)
        => await dbContext.AuditLogs
            .AsNoTracking()
            .Where(item => item.EntityName == "Contractor" && item.EntityId == contractorId)
            .OrderByDescending(item => item.ChangedAtUtc)
            .ToArrayAsync(cancellationToken);

    public void SetOriginalRowVersion(Contractor contractor, byte[] rowVersion)
    {
        dbContext.Entry(contractor).Property(item => item.RowVersion).OriginalValue = rowVersion;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            throw new ConcurrencyConflictException("The contractor was modified by another user. Refresh the data and try again.", exception);
        }
    }
}
