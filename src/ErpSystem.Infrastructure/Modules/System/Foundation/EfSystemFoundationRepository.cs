using ErpSystem.Application.Modules.System.Foundation;
using ErpSystem.Domain.Modules.System.Auditing;
using ErpSystem.Domain.Modules.System.Dictionaries;
using ErpSystem.Domain.Modules.System.Documents;
using ErpSystem.Domain.Modules.System.Identity;
using ErpSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ErpSystem.Infrastructure.Modules.System.Foundation;

public sealed class EfSystemFoundationRepository : ISystemFoundationRepository
{
    private readonly ErpSystemDbContext dbContext;

    public EfSystemFoundationRepository(ErpSystemDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Task<IReadOnlyList<User>> GetUsersAsync(CancellationToken cancellationToken = default)
        => dbContext.Users.AsNoTracking().OrderBy(item => item.Id).ToArrayAsync(cancellationToken).ContinueWith<IReadOnlyList<User>>(task => task.Result, cancellationToken);

    public Task<IReadOnlyList<Role>> GetRolesAsync(CancellationToken cancellationToken = default)
        => dbContext.Roles.AsNoTracking().OrderBy(item => item.Id).ToArrayAsync(cancellationToken).ContinueWith<IReadOnlyList<Role>>(task => task.Result, cancellationToken);

    public Task<IReadOnlyList<Permission>> GetPermissionsAsync(CancellationToken cancellationToken = default)
        => dbContext.Permissions.AsNoTracking().OrderBy(item => item.Id).ToArrayAsync(cancellationToken).ContinueWith<IReadOnlyList<Permission>>(task => task.Result, cancellationToken);

    public Task<IReadOnlyList<UserRole>> GetUserRolesAsync(CancellationToken cancellationToken = default)
        => dbContext.UserRoles.AsNoTracking().OrderBy(item => item.Id).ToArrayAsync(cancellationToken).ContinueWith<IReadOnlyList<UserRole>>(task => task.Result, cancellationToken);

    public Task<IReadOnlyList<RolePermission>> GetRolePermissionsAsync(CancellationToken cancellationToken = default)
        => dbContext.RolePermissions.AsNoTracking().OrderBy(item => item.Id).ToArrayAsync(cancellationToken).ContinueWith<IReadOnlyList<RolePermission>>(task => task.Result, cancellationToken);

    public Task<IReadOnlyList<AuditLog>> GetAuditLogsAsync(CancellationToken cancellationToken = default)
        => dbContext.AuditLogs.AsNoTracking().OrderByDescending(item => item.ChangedAtUtc).ToArrayAsync(cancellationToken).ContinueWith<IReadOnlyList<AuditLog>>(task => task.Result, cancellationToken);

    public Task<IReadOnlyList<DictionaryItem>> GetDictionaryItemsAsync(CancellationToken cancellationToken = default)
        => dbContext.DictionaryItems.AsNoTracking().OrderBy(item => item.DictionaryName).ThenBy(item => item.SortOrder).ToArrayAsync(cancellationToken).ContinueWith<IReadOnlyList<DictionaryItem>>(task => task.Result, cancellationToken);

    public Task<IReadOnlyList<Attachment>> GetAttachmentsAsync(CancellationToken cancellationToken = default)
        => dbContext.Attachments.AsNoTracking().OrderByDescending(item => item.UploadedAtUtc).ToArrayAsync(cancellationToken).ContinueWith<IReadOnlyList<Attachment>>(task => task.Result, cancellationToken);

    public Task<IReadOnlyList<DocumentHistoryEntry>> GetDocumentHistoryEntriesAsync(CancellationToken cancellationToken = default)
        => dbContext.DocumentHistoryEntries.AsNoTracking().OrderByDescending(item => item.OccurredAtUtc).ToArrayAsync(cancellationToken).ContinueWith<IReadOnlyList<DocumentHistoryEntry>>(task => task.Result, cancellationToken);

    public Task<IReadOnlyList<DocumentStatus>> GetDocumentStatusesAsync(CancellationToken cancellationToken = default)
        => dbContext.DocumentStatuses.AsNoTracking().OrderBy(item => item.ModuleKey).ThenBy(item => item.SortOrder).ToArrayAsync(cancellationToken).ContinueWith<IReadOnlyList<DocumentStatus>>(task => task.Result, cancellationToken);

    public Task<IReadOnlyList<DocumentNumberSequence>> GetDocumentNumberSequencesAsync(CancellationToken cancellationToken = default)
        => dbContext.DocumentNumberSequences.AsNoTracking().OrderBy(item => item.Key).ToArrayAsync(cancellationToken).ContinueWith<IReadOnlyList<DocumentNumberSequence>>(task => task.Result, cancellationToken);

    public Task<DocumentNumberSequence?> GetDocumentNumberSequenceAsync(string key, CancellationToken cancellationToken = default)
        => dbContext.DocumentNumberSequences.FirstOrDefaultAsync(item => item.Key == key, cancellationToken);

    public async Task<string> GenerateDocumentNumberAsync(string key, DateTime utcNow, CancellationToken cancellationToken = default)
    {
        var sequence = await dbContext.DocumentNumberSequences.FirstOrDefaultAsync(item => item.Key == key, cancellationToken);
        if (sequence is null)
        {
            throw new InvalidOperationException($"Document sequence '{key}' was not found.");
        }

        var nextNumber = sequence.GenerateNextNumber(utcNow);
        await dbContext.SaveChangesAsync(cancellationToken);
        return nextNumber;
    }
}
