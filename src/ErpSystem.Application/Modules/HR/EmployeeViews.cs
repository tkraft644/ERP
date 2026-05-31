namespace ErpSystem.Application.Modules.HR;

public sealed record EmployeeListItemView(
    int Id,
    string EmployeeNumber,
    string FullName,
    string Email,
    string? PhoneNumber,
    string? DepartmentName,
    string? PositionName,
    string? ContractType,
    bool IsActive,
    bool HasDriverProfile,
    byte[] RowVersion);

public sealed record EmploymentContractView(
    int Id,
    string ContractNumber,
    string ContractType,
    int DepartmentId,
    string DepartmentName,
    int PositionId,
    string PositionName,
    DateTime StartDate,
    DateTime? EndDate,
    decimal EmploymentRate,
    decimal? MonthlySalary,
    string? Notes);

public sealed record WorkScheduleView(
    int Id,
    string Name,
    decimal WeeklyHours,
    DateTime EffectiveFrom,
    DateTime? EffectiveTo,
    string? Notes);

public sealed record EmployeeDocumentView(
    int Id,
    string DocumentType,
    string Title,
    string? DocumentNumber,
    DateTime? IssuedAt,
    DateTime? ValidUntil,
    string? Notes,
    bool IsActive);

public sealed record DriverProfileView(
    int Id,
    string LicenseNumber,
    string? LicenseCategories,
    DateTime? LicenseValidUntil,
    string? DriverCardNumber,
    DateTime? DriverCardValidUntil,
    DateTime? AdrValidUntil);

public sealed record EmployeeLeaveRequestView(
    int Id,
    int LeaveTypeId,
    string LeaveTypeName,
    string Status,
    DateTime DateFrom,
    DateTime DateTo,
    decimal DayCount,
    string? Reason,
    string? DecisionNote,
    DateTime? DecidedAtUtc);

public sealed record EmployeeDetailsView(
    int Id,
    string EmployeeNumber,
    string FirstName,
    string LastName,
    string FullName,
    string Email,
    string? PhoneNumber,
    string? PersonalId,
    bool IsActive,
    DriverProfileView? DriverProfile,
    IReadOnlyList<EmploymentContractView> EmploymentContracts,
    IReadOnlyList<WorkScheduleView> WorkSchedules,
    IReadOnlyList<EmployeeDocumentView> Documents,
    IReadOnlyList<EmployeeLeaveRequestView> LeaveRequests,
    byte[] RowVersion);
