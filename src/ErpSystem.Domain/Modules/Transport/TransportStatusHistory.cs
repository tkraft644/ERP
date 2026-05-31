namespace ErpSystem.Domain.Modules.Transport;

public sealed class TransportStatusHistory : Common.AuditableEntity
{
    private TransportStatusHistory()
    {
    }

    public TransportStatusHistory(
        TransportOrderStatus status,
        int? changedByUserId,
        DateTime changedAtUtc,
        string? note)
    {
        Status = status;
        ChangedByUserId = changedByUserId;
        ChangedAtUtc = changedAtUtc;
        Note = note;
    }

    public int TransportOrderId { get; private set; }
    public TransportOrder TransportOrder { get; private set; } = null!;
    public TransportOrderStatus Status { get; private set; }
    public int? ChangedByUserId { get; private set; }
    public DateTime ChangedAtUtc { get; private set; }
    public string? Note { get; private set; }

    public void AssignTo(TransportOrder order)
    {
        TransportOrder = order;
        TransportOrderId = order.Id;
    }
}
