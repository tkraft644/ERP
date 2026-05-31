using ErpSystem.Application.Common;
using ErpSystem.Application.Modules.HR;
using ErpSystem.Domain.Modules.HR;
using ErpSystem.Domain.Modules.System.Auditing;

namespace ErpSystem.Tests;

public class HrServiceTests
{
    [Fact]
    public async Task CreateEmployeeAsync_ShouldRequireAtLeastOneEmploymentContract()
    {
        var service = new HrService(new TestHrRepository(), new TestCurrentUserAccessor(4));

        var request = new SaveEmployeeRequest(
            "EMP-100",
            "Marta",
            "Lis",
            "marta.lis@erpsystem.local",
            null,
            null,
            true,
            [],
            [],
            [],
            null);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.CreateEmployeeAsync(request));

        Assert.Equal("EmploymentContracts", exception.ParamName);
    }

    [Fact]
    public async Task DecideLeaveRequestAsync_ShouldRejectSubmittedAsDecisionStatus()
    {
        var repository = new TestHrRepository
        {
            LeaveRequestForUpdate = CreateLeaveRequest()
        };
        var service = new HrService(repository, new TestCurrentUserAccessor(4));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.DecideLeaveRequestAsync(
                repository.LeaveRequestForUpdate!.Id,
                new DecideLeaveRequestRequest("Submitted", "Brak decyzji", [1])));

        Assert.Contains("not a decision status", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    private static LeaveRequest CreateLeaveRequest()
    {
        var employee = new Employee("EMP-002", "Jan", "Nowak", "jan.nowak@erpsystem.local", null, null)
        {
            Id = 2
        };
        var leaveType = new LeaveType("ANNUAL", "Urlop wypoczynkowy", true, true, 10)
        {
            Id = 1
        };
        var leaveRequest = new LeaveRequest(employee.Id, leaveType.Id, new DateTime(2026, 7, 1), new DateTime(2026, 7, 5), 5m, "Wakacje")
        {
            Id = 5,
            RowVersion = [1]
        };

        typeof(LeaveRequest).GetProperty(nameof(LeaveRequest.Employee))!.SetValue(leaveRequest, employee);
        typeof(LeaveRequest).GetProperty(nameof(LeaveRequest.LeaveType))!.SetValue(leaveRequest, leaveType);
        return leaveRequest;
    }

    private sealed class TestCurrentUserAccessor : ICurrentUserAccessor
    {
        public TestCurrentUserAccessor(int? userId)
        {
            UserId = userId;
        }

        public int? UserId { get; }
    }

    private sealed class TestHrRepository : IHrRepository
    {
        public LeaveRequest? LeaveRequestForUpdate { get; set; }

        public Task<IReadOnlyList<Department>> GetDepartmentsAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Department>>([]);

        public Task<IReadOnlyList<Position>> GetPositionsAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Position>>([]);

        public Task<IReadOnlyList<LeaveType>> GetLeaveTypesAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<LeaveType>>([]);

        public Task<IReadOnlyList<Employee>> GetEmployeesAsync(bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Employee>>([]);

        public Task<Employee?> GetEmployeeAsync(int employeeId, CancellationToken cancellationToken = default)
            => Task.FromResult<Employee?>(null);

        public Task<Employee?> GetEmployeeForUpdateAsync(int employeeId, CancellationToken cancellationToken = default)
            => Task.FromResult<Employee?>(null);

        public Task AddEmployeeAsync(Employee employee, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<IReadOnlyList<LeaveRequest>> GetLeaveRequestsAsync(int? employeeId, LeaveRequestStatus? status, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<LeaveRequest>>([]);

        public Task<LeaveRequest?> GetLeaveRequestAsync(int leaveRequestId, CancellationToken cancellationToken = default)
            => Task.FromResult(LeaveRequestForUpdate);

        public Task<LeaveRequest?> GetLeaveRequestForUpdateAsync(int leaveRequestId, CancellationToken cancellationToken = default)
            => Task.FromResult(LeaveRequestForUpdate);

        public Task AddLeaveRequestAsync(LeaveRequest leaveRequest, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task AddAuditLogAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public void SetOriginalRowVersion(Employee employee, byte[] rowVersion)
        {
        }

        public void SetOriginalRowVersion(LeaveRequest leaveRequest, byte[] rowVersion)
        {
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }
}
