namespace ErpSystem.Application.Modules.System.Foundation;

public interface ISystemFoundationService
{
    Task<IReadOnlyList<UserAccessProfile>> GetUsersAsync(CancellationToken cancellationToken = default);
    Task<UserAccessProfile?> GetUserAsync(int userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MenuSectionView>> GetMenuForUserAsync(int userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PermissionView>> GetPermissionsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AuditLogView>> GetAuditLogsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DictionaryItemView>> GetDictionaryItemsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AttachmentView>> GetAttachmentsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DocumentHistoryView>> GetDocumentHistoryAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DocumentStatusView>> GetDocumentStatusesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DocumentNumberSequenceView>> GetDocumentNumberSequencesAsync(CancellationToken cancellationToken = default);
    Task<string> GenerateDocumentNumberAsync(string key, CancellationToken cancellationToken = default);
}
