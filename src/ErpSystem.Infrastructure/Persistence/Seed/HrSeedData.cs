using ErpSystem.Domain.Modules.HR;

namespace ErpSystem.Infrastructure.Persistence.Seed;

internal static class HrSeedData
{
    private static readonly DateTime SeedTimestamp = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static Department[] Departments =>
    [
        Create(new Department("LOG", "Logistyka", "Dział transportu i operacji."), 1),
        Create(new Department("HR", "HR", "Kadry i administracja pracownicza."), 2),
        Create(new Department("ADM", "Administracja", "Obsługa wewnętrzna firmy."), 3)
    ];

    public static Position[] Positions =>
    [
        Create(new Position("DRIVER", "Kierowca", "Stanowisko kierowcy zawodowego."), 1),
        Create(new Position("DISPATCHER", "Dyspozytor", "Planowanie i nadzór nad transportem."), 2),
        Create(new Position("HRSPEC", "Specjalista HR", "Obsługa procesów kadrowych."), 3)
    ];

    public static LeaveType[] LeaveTypes =>
    [
        Create(new LeaveType("ANNUAL", "Urlop wypoczynkowy", true, true, 10), 1),
        Create(new LeaveType("SICK", "Nieobecność chorobowa", true, false, 20), 2),
        Create(new LeaveType("UNPAID", "Urlop bezpłatny", false, true, 30), 3)
    ];

    public static Employee[] Employees =>
    [
        Create(new Employee("EMP-001", "Anna", "Krawczyk", "anna.krawczyk@erpsystem.local", "+48 600 101 101", "85010112345"), 1),
        Create(new Employee("EMP-002", "Jan", "Nowak", "jan.nowak@erpsystem.local", "+48 600 100 200", "87020212345"), 2),
        Create(new Employee("EMP-003", "Piotr", "Kaczmarek", "piotr.kaczmarek@erpsystem.local", "+48 600 300 400", "89030312345"), 3)
    ];

    public static EmploymentContract[] EmploymentContracts =>
    [
        Create(new EmploymentContract("CON-001", "Employment", 2, 3, new DateTime(2024, 1, 1), null, 1.0m, 9200m, "Prowadzenie procesów HR."), 1, 1),
        Create(new EmploymentContract("CON-002", "Employment", 1, 1, new DateTime(2024, 3, 1), null, 1.0m, 7800m, "Kierowca krajowy i międzynarodowy."), 2, 2),
        Create(new EmploymentContract("CON-003", "Employment", 1, 1, new DateTime(2024, 6, 1), null, 1.0m, 7600m, "Kierowca floty chłodniczej."), 3, 3)
    ];

    public static WorkSchedule[] WorkSchedules =>
    [
        Create(new WorkSchedule("Biurowy 8-16", 40m, new DateTime(2024, 1, 1), null, null), 1, 1),
        Create(new WorkSchedule("Kierowcy 4/1", 40m, new DateTime(2024, 3, 1), null, "Cykl 4 tygodnie pracy / 1 tydzień wolnego."), 2, 2),
        Create(new WorkSchedule("Kierowcy 4/1", 40m, new DateTime(2024, 6, 1), null, "Cykl 4 tygodnie pracy / 1 tydzień wolnego."), 3, 3)
    ];

    public static EmployeeDocument[] EmployeeDocuments =>
    [
        Create(new EmployeeDocument("MedicalExam", "Badania okresowe", "MED-001", new DateTime(2025, 1, 10), new DateTime(2027, 1, 10), null), 1, 2),
        Create(new EmployeeDocument("DriverQualification", "Świadectwo kwalifikacji", "DRV-QUAL-002", new DateTime(2025, 2, 1), new DateTime(2030, 2, 1), null), 2, 2),
        Create(new EmployeeDocument("DriverQualification", "Świadectwo kwalifikacji", "DRV-QUAL-003", new DateTime(2025, 2, 15), new DateTime(2030, 2, 15), null), 3, 3)
    ];

    public static DriverProfile[] DriverProfiles =>
    [
        Create(new DriverProfile("PL1234567", "C+E", new DateTime(2030, 5, 1), "CARD-002", new DateTime(2028, 5, 1), new DateTime(2027, 12, 31)), 1, 2),
        Create(new DriverProfile("PL7654321", "C+E", new DateTime(2031, 6, 1), "CARD-003", new DateTime(2028, 6, 1), null), 2, 3)
    ];

    public static LeaveRequest[] LeaveRequests =>
    [
        CreateApprovedLeaveRequest(1, 1, 1, new DateTime(2026, 7, 14), new DateTime(2026, 7, 18), 5m, "Planowany urlop letni."),
        CreateSubmittedLeaveRequest(2, 2, 1, new DateTime(2026, 8, 3), new DateTime(2026, 8, 7), 5m, "Urlop rodzinny.")
    ];

    private static LeaveRequest CreateApprovedLeaveRequest(int id, int employeeId, int leaveTypeId, DateTime dateFrom, DateTime dateTo, decimal dayCount, string reason)
    {
        var leaveRequest = new LeaveRequest(employeeId, leaveTypeId, dateFrom, dateTo, dayCount, reason);
        leaveRequest.Decide(LeaveRequestStatus.Approved, FoundationSeedData.AdminUserId, SeedTimestamp.AddDays(30), "Zatwierdzono.");
        return Create(leaveRequest, id);
    }

    private static LeaveRequest CreateSubmittedLeaveRequest(int id, int employeeId, int leaveTypeId, DateTime dateFrom, DateTime dateTo, decimal dayCount, string reason)
    {
        var leaveRequest = new LeaveRequest(employeeId, leaveTypeId, dateFrom, dateTo, dayCount, reason);
        return Create(leaveRequest, id);
    }

    private static T Create<T>(T entity, int id) where T : ErpSystem.Domain.Common.AuditableEntity
    {
        entity.Id = id;
        entity.CreatedAt = SeedTimestamp;
        entity.CreatedByUserId = FoundationSeedData.AdminUserId;
        entity.IsDeleted = false;
        entity.RowVersion = [];
        return entity;
    }

    private static T Create<T>(T entity, int id, int employeeId) where T : ErpSystem.Domain.Common.AuditableEntity
    {
        switch (entity)
        {
            case EmploymentContract contract:
                typeof(EmploymentContract).GetProperty(nameof(EmploymentContract.EmployeeId))!.SetValue(contract, employeeId);
                break;
            case WorkSchedule schedule:
                typeof(WorkSchedule).GetProperty(nameof(WorkSchedule.EmployeeId))!.SetValue(schedule, employeeId);
                break;
            case EmployeeDocument document:
                typeof(EmployeeDocument).GetProperty(nameof(EmployeeDocument.EmployeeId))!.SetValue(document, employeeId);
                break;
            case DriverProfile driverProfile:
                typeof(DriverProfile).GetProperty(nameof(DriverProfile.EmployeeId))!.SetValue(driverProfile, employeeId);
                break;
        }

        return Create(entity, id);
    }
}
