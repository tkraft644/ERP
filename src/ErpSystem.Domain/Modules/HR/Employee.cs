namespace ErpSystem.Domain.Modules.HR;

public sealed class Employee : Common.AuditableEntity
{
    private Employee()
    {
    }

    public Employee(
        string employeeNumber,
        string firstName,
        string lastName,
        string email,
        string? phoneNumber,
        string? personalId,
        bool isActive = true)
    {
        EmployeeNumber = employeeNumber;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        PersonalId = personalId;
        IsActive = isActive;
    }

    public string EmployeeNumber { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? PhoneNumber { get; private set; }
    public string? PersonalId { get; private set; }
    public bool IsActive { get; private set; }

    public List<EmploymentContract> EmploymentContracts { get; private set; } = [];
    public List<WorkSchedule> WorkSchedules { get; private set; } = [];
    public List<EmployeeDocument> Documents { get; private set; } = [];
    public List<LeaveRequest> LeaveRequests { get; private set; } = [];
    public DriverProfile? DriverProfile { get; private set; }

    public string FullName => $"{FirstName} {LastName}".Trim();

    public void Update(
        string employeeNumber,
        string firstName,
        string lastName,
        string email,
        string? phoneNumber,
        string? personalId,
        bool isActive)
    {
        EmployeeNumber = employeeNumber;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        PersonalId = personalId;
        IsActive = isActive;
    }

    public void ReplaceEmploymentContracts(IEnumerable<EmploymentContract> items)
    {
        EmploymentContracts.Clear();
        foreach (var item in items)
        {
            item.AssignTo(this);
            EmploymentContracts.Add(item);
        }
    }

    public void ReplaceWorkSchedules(IEnumerable<WorkSchedule> items)
    {
        WorkSchedules.Clear();
        foreach (var item in items)
        {
            item.AssignTo(this);
            WorkSchedules.Add(item);
        }
    }

    public void ReplaceDocuments(IEnumerable<EmployeeDocument> items)
    {
        Documents.Clear();
        foreach (var item in items)
        {
            item.AssignTo(this);
            Documents.Add(item);
        }
    }

    public void SetDriverProfile(DriverProfile? driverProfile)
    {
        if (driverProfile is null)
        {
            DriverProfile = null;
            return;
        }

        driverProfile.AssignTo(this);
        DriverProfile = driverProfile;
    }
}
