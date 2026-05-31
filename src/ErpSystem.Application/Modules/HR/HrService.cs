using System.Text.Json;
using ErpSystem.Application.Common;
using ErpSystem.Domain.Modules.HR;
using ErpSystem.Domain.Modules.System.Auditing;

namespace ErpSystem.Application.Modules.HR;

public sealed class HrService : IHrService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private static readonly IReadOnlyList<HrContractTypeOptionView> ContractTypes =
    [
        new("Employment", "Umowa o pracę"),
        new("B2B", "B2B"),
        new("Mandate", "Umowa zlecenie"),
        new("Internship", "Staż")
    ];

    private readonly IHrRepository repository;
    private readonly ICurrentUserAccessor currentUserAccessor;

    public HrService(IHrRepository repository, ICurrentUserAccessor currentUserAccessor)
    {
        this.repository = repository;
        this.currentUserAccessor = currentUserAccessor;
    }

    public async Task<HrReferenceDataView> GetReferenceDataAsync(CancellationToken cancellationToken = default)
    {
        var departments = await repository.GetDepartmentsAsync(cancellationToken);
        var positions = await repository.GetPositionsAsync(cancellationToken);
        var leaveTypes = await repository.GetLeaveTypesAsync(cancellationToken);
        var employees = await repository.GetEmployeesAsync(includeInactive: false, cancellationToken);

        return new HrReferenceDataView(
            departments.Where(item => item.IsActive).OrderBy(item => item.Name)
                .Select(item => new HrDepartmentOptionView(item.Id, item.Code, item.Name))
                .ToArray(),
            positions.Where(item => item.IsActive).OrderBy(item => item.Name)
                .Select(item => new HrPositionOptionView(item.Id, item.Code, item.Name))
                .ToArray(),
            leaveTypes.Where(item => item.IsActive).OrderBy(item => item.SortOrder).ThenBy(item => item.Name)
                .Select(item => new HrLeaveTypeOptionView(item.Id, item.Code, item.Name, item.IsPaid, item.RequiresApproval))
                .ToArray(),
            employees.OrderBy(item => item.LastName).ThenBy(item => item.FirstName)
                .Select(item => new HrEmployeeOptionView(item.Id, item.EmployeeNumber, item.FullName, item.DriverProfile is not null))
                .ToArray(),
            GetLeaveStatuses(),
            ContractTypes);
    }

    public async Task<IReadOnlyList<EmployeeListItemView>> GetEmployeesAsync(bool includeInactive, CancellationToken cancellationToken = default)
    {
        var employees = await repository.GetEmployeesAsync(includeInactive, cancellationToken);
        return employees
            .OrderBy(item => item.LastName)
            .ThenBy(item => item.FirstName)
            .Select(MapEmployeeListItem)
            .ToArray();
    }

    public async Task<EmployeeDetailsView?> GetEmployeeAsync(int employeeId, CancellationToken cancellationToken = default)
    {
        var employee = await repository.GetEmployeeAsync(employeeId, cancellationToken);
        return employee is null ? null : MapEmployeeDetails(employee);
    }

    public async Task<EmployeeDetailsView> CreateEmployeeAsync(SaveEmployeeRequest request, CancellationToken cancellationToken = default)
    {
        ValidateEmployeeRequest(request);

        var employee = BuildEmployee(request);
        await repository.AddEmployeeAsync(employee, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        var created = await repository.GetEmployeeAsync(employee.Id, cancellationToken)
            ?? throw new InvalidOperationException("Created employee could not be reloaded.");
        await repository.AddAuditLogAsync(CreateAuditLog(
            "Employee",
            created.Id,
            "Created",
            "{}",
            Serialize(MapEmployeeDetails(created)),
            $"Created employee '{created.EmployeeNumber}'."),
            cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return MapEmployeeDetails(created);
    }

    public async Task<EmployeeDetailsView?> UpdateEmployeeAsync(int employeeId, SaveEmployeeRequest request, CancellationToken cancellationToken = default)
    {
        if (request.RowVersion is null || request.RowVersion.Length == 0)
        {
            throw new ArgumentException("RowVersion is required when updating an employee.", nameof(request));
        }

        ValidateEmployeeRequest(request);
        var employee = await repository.GetEmployeeForUpdateAsync(employeeId, cancellationToken);
        if (employee is null)
        {
            return null;
        }

        repository.SetOriginalRowVersion(employee, request.RowVersion);
        var previous = MapEmployeeDetails(employee);

        employee.Update(
            NormalizeRequired(request.EmployeeNumber, nameof(request.EmployeeNumber)),
            NormalizeRequired(request.FirstName, nameof(request.FirstName)),
            NormalizeRequired(request.LastName, nameof(request.LastName)),
            NormalizeRequired(request.Email, nameof(request.Email)),
            NormalizeOptional(request.PhoneNumber),
            NormalizeOptional(request.PersonalId),
            request.IsActive);
        employee.ReplaceEmploymentContracts(BuildEmploymentContracts(request.EmploymentContracts));
        employee.ReplaceWorkSchedules(BuildWorkSchedules(request.WorkSchedules));
        employee.ReplaceDocuments(BuildEmployeeDocuments(request.Documents));
        employee.SetDriverProfile(BuildDriverProfile(request.DriverProfile));

        await repository.SaveChangesAsync(cancellationToken);
        var updated = await repository.GetEmployeeAsync(employee.Id, cancellationToken)
            ?? throw new InvalidOperationException("Updated employee could not be reloaded.");
        await repository.AddAuditLogAsync(CreateAuditLog(
            "Employee",
            updated.Id,
            "Updated",
            Serialize(previous),
            Serialize(MapEmployeeDetails(updated)),
            $"Updated employee '{updated.EmployeeNumber}'."),
            cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return MapEmployeeDetails(updated);
    }

    public async Task<IReadOnlyList<LeaveRequestListItemView>> GetLeaveRequestsAsync(int? employeeId, string? status, CancellationToken cancellationToken = default)
    {
        LeaveRequestStatus? parsedStatus = string.IsNullOrWhiteSpace(status) ? null : ParseLeaveRequestStatus(status);
        var leaveRequests = await repository.GetLeaveRequestsAsync(employeeId, parsedStatus, cancellationToken);

        return leaveRequests
            .OrderByDescending(item => item.DateFrom)
            .ThenByDescending(item => item.Id)
            .Select(MapLeaveRequest)
            .ToArray();
    }

    public async Task<LeaveRequestListItemView> CreateLeaveRequestAsync(SaveLeaveRequestRequest request, CancellationToken cancellationToken = default)
    {
        ValidateLeaveRequest(request);

        var leaveRequest = new LeaveRequest(
            request.EmployeeId,
            request.LeaveTypeId,
            request.DateFrom,
            request.DateTo,
            request.DayCount,
            NormalizeOptional(request.Reason));
        await repository.AddLeaveRequestAsync(leaveRequest, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        var created = await repository.GetLeaveRequestAsync(leaveRequest.Id, cancellationToken)
            ?? throw new InvalidOperationException("Created leave request could not be reloaded.");
        await repository.AddAuditLogAsync(CreateAuditLog(
            "LeaveRequest",
            created.Id,
            "Created",
            "{}",
            Serialize(MapLeaveRequest(created)),
            $"Created leave request for employee '{created.Employee.EmployeeNumber}'."),
            cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return MapLeaveRequest(created);
    }

    public async Task<LeaveRequestListItemView?> DecideLeaveRequestAsync(int leaveRequestId, DecideLeaveRequestRequest request, CancellationToken cancellationToken = default)
    {
        if (request.RowVersion.Length == 0)
        {
            throw new ArgumentException("RowVersion is required when deciding a leave request.", nameof(request));
        }

        var leaveRequest = await repository.GetLeaveRequestForUpdateAsync(leaveRequestId, cancellationToken);
        if (leaveRequest is null)
        {
            return null;
        }

        repository.SetOriginalRowVersion(leaveRequest, request.RowVersion);
        var targetStatus = ParseLeaveRequestStatus(request.Status);
        if (targetStatus == LeaveRequestStatus.Submitted)
        {
            throw new InvalidOperationException("Submitted is not a decision status.");
        }

        leaveRequest.Decide(targetStatus, currentUserAccessor.UserId, DateTime.UtcNow, NormalizeOptional(request.DecisionNote));
        await repository.SaveChangesAsync(cancellationToken);

        var updated = await repository.GetLeaveRequestAsync(leaveRequest.Id, cancellationToken)
            ?? throw new InvalidOperationException("Updated leave request could not be reloaded.");
        await repository.AddAuditLogAsync(CreateAuditLog(
            "LeaveRequest",
            updated.Id,
            "StatusChanged",
            "{}",
            Serialize(MapLeaveRequest(updated)),
            $"Changed leave request '{updated.Id}' status to '{updated.Status}'."),
            cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return MapLeaveRequest(updated);
    }

    private AuditLog CreateAuditLog(string entityName, int entityId, string actionName, string oldValues, string newValues, string summary)
        => new(entityName, entityId, actionName, currentUserAccessor.UserId, oldValues, newValues, summary)
        {
            ChangedAtUtc = DateTime.UtcNow
        };

    private static Employee BuildEmployee(SaveEmployeeRequest request)
    {
        var employee = new Employee(
            NormalizeRequired(request.EmployeeNumber, nameof(request.EmployeeNumber)),
            NormalizeRequired(request.FirstName, nameof(request.FirstName)),
            NormalizeRequired(request.LastName, nameof(request.LastName)),
            NormalizeRequired(request.Email, nameof(request.Email)),
            NormalizeOptional(request.PhoneNumber),
            NormalizeOptional(request.PersonalId),
            request.IsActive);
        employee.ReplaceEmploymentContracts(BuildEmploymentContracts(request.EmploymentContracts));
        employee.ReplaceWorkSchedules(BuildWorkSchedules(request.WorkSchedules));
        employee.ReplaceDocuments(BuildEmployeeDocuments(request.Documents));
        employee.SetDriverProfile(BuildDriverProfile(request.DriverProfile));
        return employee;
    }

    private static IReadOnlyList<EmploymentContract> BuildEmploymentContracts(IReadOnlyList<SaveEmploymentContractRequest> requests)
        => requests
            .OrderBy(item => item.StartDate)
            .Select(item => new EmploymentContract(
                NormalizeRequired(item.ContractNumber, nameof(item.ContractNumber)),
                NormalizeRequired(item.ContractType, nameof(item.ContractType)),
                item.DepartmentId,
                item.PositionId,
                item.StartDate,
                item.EndDate,
                item.EmploymentRate,
                item.MonthlySalary,
                NormalizeOptional(item.Notes)))
            .ToArray();

    private static IReadOnlyList<WorkSchedule> BuildWorkSchedules(IReadOnlyList<SaveWorkScheduleRequest> requests)
        => requests
            .OrderBy(item => item.EffectiveFrom)
            .Select(item => new WorkSchedule(
                NormalizeRequired(item.Name, nameof(item.Name)),
                item.WeeklyHours,
                item.EffectiveFrom,
                item.EffectiveTo,
                NormalizeOptional(item.Notes)))
            .ToArray();

    private static IReadOnlyList<EmployeeDocument> BuildEmployeeDocuments(IReadOnlyList<SaveEmployeeDocumentRequest> requests)
        => requests
            .Select(item => new EmployeeDocument(
                NormalizeRequired(item.DocumentType, nameof(item.DocumentType)),
                NormalizeRequired(item.Title, nameof(item.Title)),
                NormalizeOptional(item.DocumentNumber),
                item.IssuedAt,
                item.ValidUntil,
                NormalizeOptional(item.Notes),
                item.IsActive))
            .ToArray();

    private static DriverProfile? BuildDriverProfile(SaveDriverProfileRequest? request)
    {
        if (request is null)
        {
            return null;
        }

        return new DriverProfile(
            NormalizeRequired(request.LicenseNumber, nameof(request.LicenseNumber)),
            NormalizeOptional(request.LicenseCategories),
            request.LicenseValidUntil,
            NormalizeOptional(request.DriverCardNumber),
            request.DriverCardValidUntil,
            request.AdrValidUntil);
    }

    private static void ValidateEmployeeRequest(SaveEmployeeRequest request)
    {
        _ = NormalizeRequired(request.EmployeeNumber, nameof(request.EmployeeNumber));
        _ = NormalizeRequired(request.FirstName, nameof(request.FirstName));
        _ = NormalizeRequired(request.LastName, nameof(request.LastName));
        _ = NormalizeRequired(request.Email, nameof(request.Email));

        if (request.EmploymentContracts.Count == 0)
        {
            throw new ArgumentException("Employee must contain at least one employment contract.", nameof(request.EmploymentContracts));
        }

        if (request.EmploymentContracts.Select(item => item.ContractNumber.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).Count() != request.EmploymentContracts.Count)
        {
            throw new ArgumentException("Employment contract numbers must be unique within one employee.", nameof(request.EmploymentContracts));
        }

        foreach (var contract in request.EmploymentContracts)
        {
            _ = NormalizeRequired(contract.ContractNumber, nameof(contract.ContractNumber));
            _ = NormalizeRequired(contract.ContractType, nameof(contract.ContractType));
            if (contract.EndDate.HasValue && contract.EndDate.Value < contract.StartDate)
            {
                throw new ArgumentException("Employment contract end date cannot be earlier than start date.", nameof(request.EmploymentContracts));
            }

            if (contract.EmploymentRate <= 0m)
            {
                throw new ArgumentException("Employment rate must be greater than zero.", nameof(request.EmploymentContracts));
            }
        }

        foreach (var schedule in request.WorkSchedules)
        {
            _ = NormalizeRequired(schedule.Name, nameof(schedule.Name));
            if (schedule.WeeklyHours <= 0m)
            {
                throw new ArgumentException("Weekly hours must be greater than zero.", nameof(request.WorkSchedules));
            }

            if (schedule.EffectiveTo.HasValue && schedule.EffectiveTo.Value < schedule.EffectiveFrom)
            {
                throw new ArgumentException("Work schedule end date cannot be earlier than start date.", nameof(request.WorkSchedules));
            }
        }

        foreach (var document in request.Documents)
        {
            _ = NormalizeRequired(document.DocumentType, nameof(document.DocumentType));
            _ = NormalizeRequired(document.Title, nameof(document.Title));
        }
    }

    private static void ValidateLeaveRequest(SaveLeaveRequestRequest request)
    {
        if (request.DateTo < request.DateFrom)
        {
            throw new ArgumentException("Leave request end date cannot be earlier than start date.", nameof(request.DateTo));
        }

        if (request.DayCount <= 0m)
        {
            throw new ArgumentException("Leave request day count must be greater than zero.", nameof(request.DayCount));
        }
    }

    private static EmployeeListItemView MapEmployeeListItem(Employee employee)
    {
        var currentContract = ResolveCurrentContract(employee);

        return new EmployeeListItemView(
            employee.Id,
            employee.EmployeeNumber,
            employee.FullName,
            employee.Email,
            employee.PhoneNumber,
            currentContract?.Department.Name,
            currentContract?.Position.Name,
            currentContract?.ContractType,
            employee.IsActive,
            employee.DriverProfile is not null,
            employee.RowVersion);
    }

    private static EmployeeDetailsView MapEmployeeDetails(Employee employee)
        => new(
            employee.Id,
            employee.EmployeeNumber,
            employee.FirstName,
            employee.LastName,
            employee.FullName,
            employee.Email,
            employee.PhoneNumber,
            employee.PersonalId,
            employee.IsActive,
            employee.DriverProfile is null
                ? null
                : new DriverProfileView(
                    employee.DriverProfile.Id,
                    employee.DriverProfile.LicenseNumber,
                    employee.DriverProfile.LicenseCategories,
                    employee.DriverProfile.LicenseValidUntil,
                    employee.DriverProfile.DriverCardNumber,
                    employee.DriverProfile.DriverCardValidUntil,
                    employee.DriverProfile.AdrValidUntil),
            employee.EmploymentContracts
                .OrderByDescending(item => item.StartDate)
                .ThenByDescending(item => item.Id)
                .Select(item => new EmploymentContractView(
                    item.Id,
                    item.ContractNumber,
                    item.ContractType,
                    item.DepartmentId,
                    item.Department.Name,
                    item.PositionId,
                    item.Position.Name,
                    item.StartDate,
                    item.EndDate,
                    item.EmploymentRate,
                    item.MonthlySalary,
                    item.Notes))
                .ToArray(),
            employee.WorkSchedules
                .OrderByDescending(item => item.EffectiveFrom)
                .Select(item => new WorkScheduleView(
                    item.Id,
                    item.Name,
                    item.WeeklyHours,
                    item.EffectiveFrom,
                    item.EffectiveTo,
                    item.Notes))
                .ToArray(),
            employee.Documents
                .OrderByDescending(item => item.ValidUntil)
                .ThenBy(item => item.Title)
                .Select(item => new EmployeeDocumentView(
                    item.Id,
                    item.DocumentType,
                    item.Title,
                    item.DocumentNumber,
                    item.IssuedAt,
                    item.ValidUntil,
                    item.Notes,
                    item.IsActive))
                .ToArray(),
            employee.LeaveRequests
                .OrderByDescending(item => item.DateFrom)
                .ThenByDescending(item => item.Id)
                .Select(item => new EmployeeLeaveRequestView(
                    item.Id,
                    item.LeaveTypeId,
                    item.LeaveType.Name,
                    item.Status.ToString(),
                    item.DateFrom,
                    item.DateTo,
                    item.DayCount,
                    item.Reason,
                    item.DecisionNote,
                    item.DecidedAtUtc))
                .ToArray(),
            employee.RowVersion);

    private static LeaveRequestListItemView MapLeaveRequest(LeaveRequest leaveRequest)
        => new(
            leaveRequest.Id,
            leaveRequest.EmployeeId,
            leaveRequest.Employee.EmployeeNumber,
            leaveRequest.Employee.FullName,
            leaveRequest.LeaveTypeId,
            leaveRequest.LeaveType.Name,
            leaveRequest.Status.ToString(),
            leaveRequest.DateFrom,
            leaveRequest.DateTo,
            leaveRequest.DayCount,
            leaveRequest.Reason,
            leaveRequest.DecisionNote,
            leaveRequest.DecidedAtUtc,
            leaveRequest.RowVersion);

    private static EmploymentContract? ResolveCurrentContract(Employee employee)
        => employee.EmploymentContracts
            .OrderByDescending(item => item.EndDate.HasValue ? 0 : 1)
            .ThenByDescending(item => item.StartDate)
            .ThenByDescending(item => item.Id)
            .FirstOrDefault();

    private static IReadOnlyList<HrLeaveStatusOptionView> GetLeaveStatuses()
        => Enum.GetValues<LeaveRequestStatus>()
            .Select(item => new HrLeaveStatusOptionView(item.ToString(), item switch
            {
                LeaveRequestStatus.Submitted => "Złożony",
                LeaveRequestStatus.Approved => "Zatwierdzony",
                LeaveRequestStatus.Rejected => "Odrzucony",
                LeaveRequestStatus.Cancelled => "Anulowany",
                _ => item.ToString()
            }))
            .ToArray();

    private static LeaveRequestStatus ParseLeaveRequestStatus(string value)
    {
        if (!Enum.TryParse<LeaveRequestStatus>(value, ignoreCase: true, out var parsed))
        {
            throw new ArgumentException($"Unknown leave request status '{value}'.", nameof(value));
        }

        return parsed;
    }

    private static string NormalizeRequired(string? value, string fieldName)
    {
        if (value is null)
        {
            throw new ArgumentException($"{fieldName} is required.", fieldName);
        }

        var normalized = value.Trim();
        if (normalized.Length == 0)
        {
            throw new ArgumentException($"{fieldName} is required.", fieldName);
        }

        return normalized;
    }

    private static string? NormalizeOptional(string? value)
    {
        if (value is null)
        {
            return null;
        }

        var normalized = value.Trim();
        return normalized.Length == 0 ? null : normalized;
    }

    private static string Serialize<T>(T value) => JsonSerializer.Serialize(value, SerializerOptions);
}
