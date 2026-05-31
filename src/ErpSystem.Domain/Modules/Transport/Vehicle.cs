namespace ErpSystem.Domain.Modules.Transport;

public sealed class Vehicle : Common.AuditableEntity
{
    private Vehicle()
    {
    }

    public Vehicle(
        string registrationNumber,
        string brand,
        string model,
        decimal? payloadTons,
        int? carrierId,
        bool isActive = true)
    {
        RegistrationNumber = registrationNumber;
        Brand = brand;
        Model = model;
        PayloadTons = payloadTons;
        CarrierId = carrierId;
        IsActive = isActive;
    }

    public string RegistrationNumber { get; private set; } = string.Empty;
    public string Brand { get; private set; } = string.Empty;
    public string Model { get; private set; } = string.Empty;
    public decimal? PayloadTons { get; private set; }
    public int? CarrierId { get; private set; }
    public Carrier? Carrier { get; private set; }
    public bool IsActive { get; private set; }
}
