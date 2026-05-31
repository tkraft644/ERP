namespace ErpSystem.Domain.Modules.System.Auditing;

public sealed class AuditLog : Common.AuditableEntity
{
    public AuditLog(
        string entityName,
        int entityId,
        string actionName,
        int? changedByUserId,
        string oldValues,
        string newValues,
        string summary)
    {
        EntityName = entityName;
        EntityId = entityId;
        ActionName = actionName;
        ChangedByUserId = changedByUserId;
        OldValues = oldValues;
        NewValues = newValues;
        Summary = summary;
    }

    public string EntityName { get; set; }
    public int EntityId { get; set; }
    public string ActionName { get; set; }
    public int? ChangedByUserId { get; set; }
    public DateTime ChangedAtUtc { get; set; }
    public string OldValues { get; set; }
    public string NewValues { get; set; }
    public string Summary { get; set; }
}
