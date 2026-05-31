namespace ErpSystem.Domain.Modules.Transport;

public enum TransportOrderStatus
{
    New = 1,
    Accepted = 2,
    Planned = 3,
    InProgress = 4,
    Loaded = 5,
    Delivered = 6,
    Closed = 7,
    Cancelled = 8
}
