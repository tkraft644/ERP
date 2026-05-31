namespace ErpSystem.Application.Modules.System.Foundation;

public sealed record AttachmentView(
    int AttachmentId,
    string OwnerEntityName,
    int OwnerEntityId,
    string FileName,
    string ContentType,
    long SizeBytes,
    DateTime UploadedAtUtc);
