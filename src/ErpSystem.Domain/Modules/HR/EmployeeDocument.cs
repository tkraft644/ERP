namespace ErpSystem.Domain.Modules.HR;

public sealed class EmployeeDocument : Common.AuditableEntity
{
    private EmployeeDocument()
    {
    }

    public EmployeeDocument(
        string documentType,
        string title,
        string? documentNumber,
        DateTime? issuedAt,
        DateTime? validUntil,
        string? notes,
        bool isActive = true)
    {
        DocumentType = documentType;
        Title = title;
        DocumentNumber = documentNumber;
        IssuedAt = issuedAt;
        ValidUntil = validUntil;
        Notes = notes;
        IsActive = isActive;
    }

    public int EmployeeId { get; private set; }
    public Employee Employee { get; private set; } = null!;
    public string DocumentType { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string? DocumentNumber { get; private set; }
    public DateTime? IssuedAt { get; private set; }
    public DateTime? ValidUntil { get; private set; }
    public string? Notes { get; private set; }
    public bool IsActive { get; private set; }

    internal void AssignTo(Employee employee)
    {
        Employee = employee;
        EmployeeId = employee.Id;
    }
}
