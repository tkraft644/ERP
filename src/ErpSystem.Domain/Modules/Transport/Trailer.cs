namespace ErpSystem.Domain.Modules.Transport;

public sealed class Trailer : Common.AuditableEntity
{
    private Trailer()
    {
    }

    public Trailer(
        string registrationNumber,
        string trailerType,
        decimal? payloadTons,
        int? carrierId,
        bool isActive = true)
    {
        RegistrationNumber = registrationNumber;
        TrailerType = trailerType;
        PayloadTons = payloadTons;
        CarrierId = carrierId;
        IsActive = isActive;
    }

    public string RegistrationNumber { get; private set; } = string.Empty;
    public string TrailerType { get; private set; } = string.Empty;
    public decimal? PayloadTons { get; private set; }
    public int? CarrierId { get; private set; }
    public Carrier? Carrier { get; private set; }
    public bool IsActive { get; private set; }
}
