namespace ErpSystem.Application.Modules.System.Foundation;

public sealed record AuditLogView(
    int AuditLogId,
    string EntityName,
    int EntityId,
    string ActionName,
    int? ChangedByUserId,
    DateTime ChangedAtUtc,
    string Summary);
