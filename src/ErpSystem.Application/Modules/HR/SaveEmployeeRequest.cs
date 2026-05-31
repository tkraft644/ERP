namespace ErpSystem.Application.Modules.HR;

public sealed record SaveEmployeeRequest(
    string EmployeeNumber,
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    string? PersonalId,
    bool IsActive,
    IReadOnlyList<SaveEmploymentContractRequest> EmploymentContracts,
    IReadOnlyList<SaveWorkScheduleRequest> WorkSchedules,
    IReadOnlyList<SaveEmployeeDocumentRequest> Documents,
    SaveDriverProfileRequest? DriverProfile,
    byte[]? RowVersion = null);

public sealed record SaveEmploymentContractRequest(
    string ContractNumber,
    string ContractType,
    int DepartmentId,
    int PositionId,
    DateTime StartDate,
    DateTime? EndDate,
    decimal EmploymentRate,
    decimal? MonthlySalary,
    string? Notes);

public sealed record SaveWorkScheduleRequest(
    string Name,
    decimal WeeklyHours,
    DateTime EffectiveFrom,
    DateTime? EffectiveTo,
    string? Notes);

public sealed record SaveEmployeeDocumentRequest(
    string DocumentType,
    string Title,
    string? DocumentNumber,
    DateTime? IssuedAt,
    DateTime? ValidUntil,
    string? Notes,
    bool IsActive);

public sealed record SaveDriverProfileRequest(
    string LicenseNumber,
    string? LicenseCategories,
    DateTime? LicenseValidUntil,
    string? DriverCardNumber,
    DateTime? DriverCardValidUntil,
    DateTime? AdrValidUntil);
