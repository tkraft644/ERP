using ErpSystem.Domain.Modules.HR;
using ErpSystem.Domain.Modules.System.Auditing;

namespace ErpSystem.Application.Modules.HR;

public interface IHrRepository
{
    Task<IReadOnlyList<Department>> GetDepartmentsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Position>> GetPositionsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LeaveType>> GetLeaveTypesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Employee>> GetEmployeesAsync(bool includeInactive, CancellationToken cancellationToken = default);
    Task<Employee?> GetEmployeeAsync(int employeeId, CancellationToken cancellationToken = default);
    Task<Employee?> GetEmployeeForUpdateAsync(int employeeId, CancellationToken cancellationToken = default);
    Task AddEmployeeAsync(Employee employee, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LeaveRequest>> GetLeaveRequestsAsync(int? employeeId, LeaveRequestStatus? status, CancellationToken cancellationToken = default);
    Task<LeaveRequest?> GetLeaveRequestAsync(int leaveRequestId, CancellationToken cancellationToken = default);
    Task<LeaveRequest?> GetLeaveRequestForUpdateAsync(int leaveRequestId, CancellationToken cancellationToken = default);
    Task AddLeaveRequestAsync(LeaveRequest leaveRequest, CancellationToken cancellationToken = default);
    Task AddAuditLogAsync(AuditLog auditLog, CancellationToken cancellationToken = default);
    void SetOriginalRowVersion(Employee employee, byte[] rowVersion);
    void SetOriginalRowVersion(LeaveRequest leaveRequest, byte[] rowVersion);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
