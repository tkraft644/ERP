namespace ErpSystem.Domain.Modules.System.Documents;

public sealed class DocumentHistoryEntry : Common.AuditableEntity
{
    public DocumentHistoryEntry(
        string documentType,
        int documentId,
        string entryType,
        string description,
        int? actorUserId,
        string? statusCode = null)
    {
        DocumentType = documentType;
        DocumentId = documentId;
        EntryType = entryType;
        Description = description;
        ActorUserId = actorUserId;
        StatusCode = statusCode;
    }

    public string DocumentType { get; set; }
    public int DocumentId { get; set; }
    public string EntryType { get; set; }
    public string Description { get; set; }
    public int? ActorUserId { get; set; }
    public string? StatusCode { get; set; }
    public DateTime OccurredAtUtc { get; set; }
}
