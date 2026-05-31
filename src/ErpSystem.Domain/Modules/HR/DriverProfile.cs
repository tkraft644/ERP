namespace ErpSystem.Domain.Modules.HR;

public sealed class DriverProfile : Common.AuditableEntity
{
    private DriverProfile()
    {
    }

    public DriverProfile(
        string licenseNumber,
        string? licenseCategories,
        DateTime? licenseValidUntil,
        string? driverCardNumber,
        DateTime? driverCardValidUntil,
        DateTime? adrValidUntil)
    {
        LicenseNumber = licenseNumber;
        LicenseCategories = licenseCategories;
        LicenseValidUntil = licenseValidUntil;
        DriverCardNumber = driverCardNumber;
        DriverCardValidUntil = driverCardValidUntil;
        AdrValidUntil = adrValidUntil;
    }

    public int EmployeeId { get; private set; }
    public Employee Employee { get; private set; } = null!;
    public string LicenseNumber { get; private set; } = string.Empty;
    public string? LicenseCategories { get; private set; }
    public DateTime? LicenseValidUntil { get; private set; }
    public string? DriverCardNumber { get; private set; }
    public DateTime? DriverCardValidUntil { get; private set; }
    public DateTime? AdrValidUntil { get; private set; }

    internal void AssignTo(Employee employee)
    {
        Employee = employee;
        EmployeeId = employee.Id;
    }
}
