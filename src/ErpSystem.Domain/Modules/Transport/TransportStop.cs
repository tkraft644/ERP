using ErpSystem.Domain.Modules.Contractors;

namespace ErpSystem.Domain.Modules.Transport;

public sealed class TransportStop : Common.AuditableEntity
{
    private TransportStop()
    {
    }

    public TransportStop(
        int sequence,
        TransportStopType stopType,
        int? contractorId,
        string name,
        string countryCode,
        string city,
        string addressLine,
        DateTime? plannedAtUtc,
        DateTime? actualAtUtc,
        string? notes)
    {
        Sequence = sequence;
        StopType = stopType;
        ContractorId = contractorId;
        Name = name;
        CountryCode = countryCode;
        City = city;
        AddressLine = addressLine;
        PlannedAtUtc = plannedAtUtc;
        ActualAtUtc = actualAtUtc;
        Notes = notes;
    }

    public int TransportOrderId { get; private set; }
    public TransportOrder TransportOrder { get; private set; } = null!;
    public int Sequence { get; private set; }
    public TransportStopType StopType { get; private set; }
    public int? ContractorId { get; private set; }
    public Contractor? Contractor { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string CountryCode { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string AddressLine { get; private set; } = string.Empty;
    public DateTime? PlannedAtUtc { get; private set; }
    public DateTime? ActualAtUtc { get; private set; }
    public string? Notes { get; private set; }

    internal void AssignTo(TransportOrder order)
    {
        TransportOrder = order;
        TransportOrderId = order.Id;
    }
}
