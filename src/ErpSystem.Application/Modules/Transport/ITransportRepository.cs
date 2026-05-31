using ErpSystem.Domain.Modules.Contractors;
using ErpSystem.Domain.Modules.HR;
using ErpSystem.Domain.Modules.System.Auditing;
using ErpSystem.Domain.Modules.Transport;

namespace ErpSystem.Application.Modules.Transport;

public interface ITransportRepository
{
    Task<IReadOnlyList<Contractor>> GetPartnersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Carrier>> GetCarriersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Vehicle>> GetVehiclesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Trailer>> GetTrailersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DriverProfile>> GetDriversAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TransportOrder>> GetOrdersAsync(CancellationToken cancellationToken = default);
    Task<TransportOrder?> GetOrderAsync(int orderId, CancellationToken cancellationToken = default);
    Task<TransportOrder?> GetOrderForUpdateAsync(int orderId, CancellationToken cancellationToken = default);
    Task AddOrderAsync(TransportOrder order, CancellationToken cancellationToken = default);
    Task AddStatusHistoryAsync(TransportStatusHistory historyEntry, CancellationToken cancellationToken = default);
    Task AddAuditLogAsync(AuditLog auditLog, CancellationToken cancellationToken = default);
    Task<string> GenerateDocumentNumberAsync(string key, DateTime utcNow, CancellationToken cancellationToken = default);
    void SetOriginalRowVersion(TransportOrder order, byte[] rowVersion);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
