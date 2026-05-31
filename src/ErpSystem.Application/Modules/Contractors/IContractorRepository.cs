using ErpSystem.Domain.Modules.Contractors;
using ErpSystem.Domain.Modules.System.Auditing;

namespace ErpSystem.Application.Modules.Contractors;

public interface IContractorRepository
{
    Task<IReadOnlyList<Contractor>> GetContractorsAsync(bool includeInactive, CancellationToken cancellationToken = default);
    Task<Contractor?> GetContractorAsync(int contractorId, CancellationToken cancellationToken = default);
    Task<Contractor?> GetContractorForUpdateAsync(int contractorId, CancellationToken cancellationToken = default);
    Task AddContractorAsync(Contractor contractor, CancellationToken cancellationToken = default);
    Task AddAuditLogAsync(AuditLog auditLog, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AuditLog>> GetContractorHistoryAsync(int contractorId, CancellationToken cancellationToken = default);
    void SetOriginalRowVersion(Contractor contractor, byte[] rowVersion);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
