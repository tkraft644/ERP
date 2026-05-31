namespace ErpSystem.Domain.Modules.HR;

public sealed class WorkSchedule : Common.AuditableEntity
{
    private WorkSchedule()
    {
    }

    public WorkSchedule(
        string name,
        decimal weeklyHours,
        DateTime effectiveFrom,
        DateTime? effectiveTo,
        string? notes)
    {
        Name = name;
        WeeklyHours = weeklyHours;
        EffectiveFrom = effectiveFrom;
        EffectiveTo = effectiveTo;
        Notes = notes;
    }

    public int EmployeeId { get; private set; }
    public Employee Employee { get; private set; } = null!;
    public string Name { get; private set; } = string.Empty;
    public decimal WeeklyHours { get; private set; }
    public DateTime EffectiveFrom { get; private set; }
    public DateTime? EffectiveTo { get; private set; }
    public string? Notes { get; private set; }

    internal void AssignTo(Employee employee)
    {
        Employee = employee;
        EmployeeId = employee.Id;
    }
}
