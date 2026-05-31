using ErpSystem.Domain.Modules.System.Auditing;
using ErpSystem.Domain.Modules.System.Dictionaries;
using ErpSystem.Domain.Modules.System.Documents;
using ErpSystem.Domain.Modules.System.Identity;

namespace ErpSystem.Application.Modules.System.Foundation;

public interface ISystemFoundationRepository
{
    Task<IReadOnlyList<User>> GetUsersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Role>> GetRolesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Permission>> GetPermissionsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserRole>> GetUserRolesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RolePermission>> GetRolePermissionsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AuditLog>> GetAuditLogsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DictionaryItem>> GetDictionaryItemsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Attachment>> GetAttachmentsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DocumentHistoryEntry>> GetDocumentHistoryEntriesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DocumentStatus>> GetDocumentStatusesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DocumentNumberSequence>> GetDocumentNumberSequencesAsync(CancellationToken cancellationToken = default);
    Task<DocumentNumberSequence?> GetDocumentNumberSequenceAsync(string key, CancellationToken cancellationToken = default);
    Task<string> GenerateDocumentNumberAsync(string key, DateTime utcNow, CancellationToken cancellationToken = default);
}
