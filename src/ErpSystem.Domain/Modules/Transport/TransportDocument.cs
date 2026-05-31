namespace ErpSystem.Domain.Modules.Transport;

public sealed class TransportDocument : Common.AuditableEntity
{
    private TransportDocument()
    {
    }

    public TransportDocument(
        TransportDocumentType documentType,
        string? documentNumber,
        string? fileName,
        DateTime? issuedAtUtc,
        DateTime? receivedAtUtc,
        bool isRequired)
    {
        DocumentType = documentType;
        DocumentNumber = documentNumber;
        FileName = fileName;
        IssuedAtUtc = issuedAtUtc;
        ReceivedAtUtc = receivedAtUtc;
        IsRequired = isRequired;
    }

    public int TransportOrderId { get; private set; }
    public TransportOrder TransportOrder { get; private set; } = null!;
    public TransportDocumentType DocumentType { get; private set; }
    public string? DocumentNumber { get; private set; }
    public string? FileName { get; private set; }
    public DateTime? IssuedAtUtc { get; private set; }
    public DateTime? ReceivedAtUtc { get; private set; }
    public bool IsRequired { get; private set; }

    internal void AssignTo(TransportOrder order)
    {
        TransportOrder = order;
        TransportOrderId = order.Id;
    }
}
