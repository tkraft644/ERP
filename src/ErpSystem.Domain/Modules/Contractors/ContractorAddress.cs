namespace ErpSystem.Domain.Modules.Contractors;

public sealed class ContractorAddress : Common.AuditableEntity
{
    private ContractorAddress()
    {
    }

    public ContractorAddress(
        ContractorAddressKind kind,
        string label,
        string countryCode,
        string postalCode,
        string city,
        string street,
        string buildingNumber,
        string? apartmentNumber,
        bool isPrimary = false)
    {
        Kind = kind;
        Label = label;
        CountryCode = countryCode;
        PostalCode = postalCode;
        City = city;
        Street = street;
        BuildingNumber = buildingNumber;
        ApartmentNumber = apartmentNumber;
        IsPrimary = isPrimary;
    }

    public int ContractorId { get; private set; }
    public Contractor Contractor { get; private set; } = null!;
    public ContractorAddressKind Kind { get; private set; }
    public string Label { get; private set; } = string.Empty;
    public string CountryCode { get; private set; } = string.Empty;
    public string PostalCode { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string Street { get; private set; } = string.Empty;
    public string BuildingNumber { get; private set; } = string.Empty;
    public string? ApartmentNumber { get; private set; }
    public bool IsPrimary { get; private set; }

    internal void AssignTo(Contractor contractor)
    {
        Contractor = contractor;
        ContractorId = contractor.Id;
    }
}
