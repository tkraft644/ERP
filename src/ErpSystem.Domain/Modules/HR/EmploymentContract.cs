namespace ErpSystem.Domain.Modules.HR;

public sealed class EmploymentContract : Common.AuditableEntity
{
    private EmploymentContract()
    {
    }

    public EmploymentContract(
        string contractNumber,
        string contractType,
        int departmentId,
        int positionId,
        DateTime startDate,
        DateTime? endDate,
        decimal employmentRate,
        decimal? monthlySalary,
        string? notes)
    {
        ContractNumber = contractNumber;
        ContractType = contractType;
        DepartmentId = departmentId;
        PositionId = positionId;
        StartDate = startDate;
        EndDate = endDate;
        EmploymentRate = employmentRate;
        MonthlySalary = monthlySalary;
        Notes = notes;
    }

    public int EmployeeId { get; private set; }
    public Employee Employee { get; private set; } = null!;
    public string ContractNumber { get; private set; } = string.Empty;
    public string ContractType { get; private set; } = string.Empty;
    public int DepartmentId { get; private set; }
    public Department Department { get; private set; } = null!;
    public int PositionId { get; private set; }
    public Position Position { get; private set; } = null!;
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public decimal EmploymentRate { get; private set; }
    public decimal? MonthlySalary { get; private set; }
    public string? Notes { get; private set; }

    internal void AssignTo(Employee employee)
    {
        Employee = employee;
        EmployeeId = employee.Id;
    }
}
