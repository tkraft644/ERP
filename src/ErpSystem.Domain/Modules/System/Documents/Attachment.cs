namespace ErpSystem.Domain.Modules.System.Documents;

public sealed class Attachment : Common.AuditableEntity
{
    public Attachment(
        string ownerEntityName,
        int ownerEntityId,
        string fileName,
        string contentType,
        string storagePath,
        long sizeBytes,
        int uploadedByUserId)
    {
        OwnerEntityName = ownerEntityName;
        OwnerEntityId = ownerEntityId;
        FileName = fileName;
        ContentType = contentType;
        StoragePath = storagePath;
        SizeBytes = sizeBytes;
        UploadedByUserId = uploadedByUserId;
    }

    public string OwnerEntityName { get; set; }
    public int OwnerEntityId { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public string StoragePath { get; set; }
    public long SizeBytes { get; set; }
    public int UploadedByUserId { get; set; }
    public DateTime UploadedAtUtc { get; set; }
}
