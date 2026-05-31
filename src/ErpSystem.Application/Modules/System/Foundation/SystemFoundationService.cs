using ErpSystem.Domain.Modules.System.Documents;
using ErpSystem.Domain.Modules.System.Identity;

namespace ErpSystem.Application.Modules.System.Foundation;

public sealed class SystemFoundationService : ISystemFoundationService
{
    private readonly ISystemFoundationRepository repository;
    private readonly IMenuBuilder menuBuilder;

    public SystemFoundationService(ISystemFoundationRepository repository, IMenuBuilder menuBuilder)
    {
        this.repository = repository;
        this.menuBuilder = menuBuilder;
    }

    public async Task<IReadOnlyList<UserAccessProfile>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await repository.GetUsersAsync(cancellationToken);
        var roles = await repository.GetRolesAsync(cancellationToken);
        var permissions = await repository.GetPermissionsAsync(cancellationToken);
        var userRoles = await repository.GetUserRolesAsync(cancellationToken);
        var rolePermissions = await repository.GetRolePermissionsAsync(cancellationToken);

        return users.Select(user => BuildUserAccessProfile(user, roles, permissions, userRoles, rolePermissions)).ToArray();
    }

    public async Task<UserAccessProfile?> GetUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        var profiles = await GetUsersAsync(cancellationToken);
        return profiles.FirstOrDefault(profile => profile.UserId == userId);
    }

    public async Task<IReadOnlyList<MenuSectionView>> GetMenuForUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        var profile = await GetUserAsync(userId, cancellationToken);
        return profile is null ? [] : menuBuilder.Build(profile.Permissions);
    }

    public async Task<IReadOnlyList<PermissionView>> GetPermissionsAsync(CancellationToken cancellationToken = default)
    {
        var permissions = await repository.GetPermissionsAsync(cancellationToken);

        return permissions
            .OrderBy(item => item.Name)
            .Select(item => new PermissionView(item.Name, item.ModuleKey, item.Resource, item.Action, item.Description))
            .ToArray();
    }

    public async Task<IReadOnlyList<AuditLogView>> GetAuditLogsAsync(CancellationToken cancellationToken = default)
    {
        var auditLogs = await repository.GetAuditLogsAsync(cancellationToken);

        return auditLogs
            .OrderByDescending(item => item.ChangedAtUtc)
            .Select(item => new AuditLogView(
                item.Id,
                item.EntityName,
                item.EntityId,
                item.ActionName,
                item.ChangedByUserId,
                item.ChangedAtUtc,
                item.Summary))
            .ToArray();
    }

    public async Task<IReadOnlyList<DictionaryItemView>> GetDictionaryItemsAsync(CancellationToken cancellationToken = default)
    {
        var dictionaryItems = await repository.GetDictionaryItemsAsync(cancellationToken);

        return dictionaryItems
            .OrderBy(item => item.DictionaryName)
            .ThenBy(item => item.SortOrder)
            .Select(item => new DictionaryItemView(
                item.Id,
                item.DictionaryName,
                item.Code,
                item.Name,
                item.Value,
                item.SortOrder,
                item.IsActive))
            .ToArray();
    }

    public async Task<IReadOnlyList<AttachmentView>> GetAttachmentsAsync(CancellationToken cancellationToken = default)
    {
        var attachments = await repository.GetAttachmentsAsync(cancellationToken);

        return attachments
            .OrderByDescending(item => item.UploadedAtUtc)
            .Select(item => new AttachmentView(
                item.Id,
                item.OwnerEntityName,
                item.OwnerEntityId,
                item.FileName,
                item.ContentType,
                item.SizeBytes,
                item.UploadedAtUtc))
            .ToArray();
    }

    public async Task<IReadOnlyList<DocumentHistoryView>> GetDocumentHistoryAsync(CancellationToken cancellationToken = default)
    {
        var historyEntries = await repository.GetDocumentHistoryEntriesAsync(cancellationToken);

        return historyEntries
            .OrderByDescending(item => item.OccurredAtUtc)
            .Select(item => new DocumentHistoryView(
                item.Id,
                item.DocumentType,
                item.DocumentId,
                item.EntryType,
                item.Description,
                item.StatusCode,
                item.OccurredAtUtc))
            .ToArray();
    }

    public async Task<IReadOnlyList<DocumentStatusView>> GetDocumentStatusesAsync(CancellationToken cancellationToken = default)
    {
        var statuses = await repository.GetDocumentStatusesAsync(cancellationToken);

        return statuses
            .OrderBy(item => item.ModuleKey)
            .ThenBy(item => item.SortOrder)
            .Select(item => new DocumentStatusView(
                item.Id,
                item.ModuleKey,
                item.Code,
                item.Name,
                item.SortOrder,
                item.IsTerminal))
            .ToArray();
    }

    public async Task<IReadOnlyList<DocumentNumberSequenceView>> GetDocumentNumberSequencesAsync(CancellationToken cancellationToken = default)
    {
        var sequences = await repository.GetDocumentNumberSequencesAsync(cancellationToken);

        return sequences
            .OrderBy(item => item.Key)
            .Select(item => MapSequence(item))
            .ToArray();
    }

    public Task<string> GenerateDocumentNumberAsync(string key, CancellationToken cancellationToken = default)
    {
        return repository.GenerateDocumentNumberAsync(key, DateTime.UtcNow, cancellationToken);
    }

    private static UserAccessProfile BuildUserAccessProfile(
        User user,
        IReadOnlyList<Role> roles,
        IReadOnlyList<Permission> permissions,
        IReadOnlyList<UserRole> userRoles,
        IReadOnlyList<RolePermission> rolePermissions)
    {
        var assignedRoleIds = userRoles
            .Where(item => item.UserId == user.Id)
            .Select(item => item.RoleId)
            .ToHashSet();

        var roleNames = roles
            .Where(role => assignedRoleIds.Contains(role.Id))
            .Select(role => role.Name)
            .OrderBy(name => name)
            .ToArray();

        var allowedPermissionIds = rolePermissions
            .Where(item => assignedRoleIds.Contains(item.RoleId))
            .Select(item => item.PermissionId)
            .ToHashSet();

        var permissionNames = permissions
            .Where(item => allowedPermissionIds.Contains(item.Id))
            .Select(item => item.Name)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name)
            .ToArray();

        return new UserAccessProfile(
            user.Id,
            user.UserName,
            user.DisplayName,
            user.Email,
            user.IsActive,
            roleNames,
            permissionNames);
    }

    private static DocumentNumberSequenceView MapSequence(DocumentNumberSequence sequence)
    {
        return new DocumentNumberSequenceView(
            sequence.Id,
            sequence.Key,
            sequence.Prefix,
            sequence.CurrentNumber,
            sequence.Padding,
            sequence.ResetPolicy.ToString());
    }
}
