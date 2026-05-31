namespace ErpSystem.Application.Modules.HR;

public interface IHrService
{
    Task<HrReferenceDataView> GetReferenceDataAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EmployeeListItemView>> GetEmployeesAsync(bool includeInactive, CancellationToken cancellationToken = default);
    Task<EmployeeDetailsView?> GetEmployeeAsync(int employeeId, CancellationToken cancellationToken = default);
    Task<EmployeeDetailsView> CreateEmployeeAsync(SaveEmployeeRequest request, CancellationToken cancellationToken = default);
    Task<EmployeeDetailsView?> UpdateEmployeeAsync(int employeeId, SaveEmployeeRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LeaveRequestListItemView>> GetLeaveRequestsAsync(int? employeeId, string? status, CancellationToken cancellationToken = default);
    Task<LeaveRequestListItemView> CreateLeaveRequestAsync(SaveLeaveRequestRequest request, CancellationToken cancellationToken = default);
    Task<LeaveRequestListItemView?> DecideLeaveRequestAsync(int leaveRequestId, DecideLeaveRequestRequest request, CancellationToken cancellationToken = default);
}
