using ErpSystem.Application.Common;
using ErpSystem.Application.Modules.HR;
using ErpSystem.Domain.Modules.HR;
using ErpSystem.Domain.Modules.System.Auditing;
using ErpSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ErpSystem.Infrastructure.Modules.HR;

public sealed class EfHrRepository : IHrRepository
{
    private readonly ErpSystemDbContext dbContext;

    public EfHrRepository(ErpSystemDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Department>> GetDepartmentsAsync(CancellationToken cancellationToken = default)
        => await dbContext.Departments.AsNoTracking().OrderBy(item => item.Name).ToArrayAsync(cancellationToken);

    public async Task<IReadOnlyList<Position>> GetPositionsAsync(CancellationToken cancellationToken = default)
        => await dbContext.Positions.AsNoTracking().OrderBy(item => item.Name).ToArrayAsync(cancellationToken);

    public async Task<IReadOnlyList<LeaveType>> GetLeaveTypesAsync(CancellationToken cancellationToken = default)
        => await dbContext.LeaveTypes.AsNoTracking().OrderBy(item => item.SortOrder).ThenBy(item => item.Name).ToArrayAsync(cancellationToken);

    public async Task<IReadOnlyList<Employee>> GetEmployeesAsync(bool includeInactive, CancellationToken cancellationToken = default)
    {
        var query = BuildEmployeeQuery(asNoTracking: true).AsQueryable();
        if (!includeInactive)
        {
            query = query.Where(item => item.IsActive);
        }

        return await query.OrderBy(item => item.LastName).ThenBy(item => item.FirstName).ToArrayAsync(cancellationToken);
    }

    public Task<Employee?> GetEmployeeAsync(int employeeId, CancellationToken cancellationToken = default)
        => BuildEmployeeQuery(asNoTracking: true).FirstOrDefaultAsync(item => item.Id == employeeId, cancellationToken);

    public Task<Employee?> GetEmployeeForUpdateAsync(int employeeId, CancellationToken cancellationToken = default)
        => BuildEmployeeQuery(asNoTracking: false).FirstOrDefaultAsync(item => item.Id == employeeId, cancellationToken);

    public Task AddEmployeeAsync(Employee employee, CancellationToken cancellationToken = default)
        => dbContext.Employees.AddAsync(employee, cancellationToken).AsTask();

    public async Task<IReadOnlyList<LeaveRequest>> GetLeaveRequestsAsync(int? employeeId, LeaveRequestStatus? status, CancellationToken cancellationToken = default)
    {
        var query = dbContext.LeaveRequests
            .AsNoTracking()
            .Include(item => item.Employee)
            .Include(item => item.LeaveType)
            .AsQueryable();

        if (employeeId.HasValue)
        {
            query = query.Where(item => item.EmployeeId == employeeId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(item => item.Status == status.Value);
        }

        return await query.OrderByDescending(item => item.DateFrom).ThenByDescending(item => item.Id).ToArrayAsync(cancellationToken);
    }

    public Task<LeaveRequest?> GetLeaveRequestAsync(int leaveRequestId, CancellationToken cancellationToken = default)
        => BuildLeaveRequestQuery(asNoTracking: true).FirstOrDefaultAsync(item => item.Id == leaveRequestId, cancellationToken);

    public Task<LeaveRequest?> GetLeaveRequestForUpdateAsync(int leaveRequestId, CancellationToken cancellationToken = default)
        => BuildLeaveRequestQuery(asNoTracking: false).FirstOrDefaultAsync(item => item.Id == leaveRequestId, cancellationToken);

    public Task AddLeaveRequestAsync(LeaveRequest leaveRequest, CancellationToken cancellationToken = default)
        => dbContext.LeaveRequests.AddAsync(leaveRequest, cancellationToken).AsTask();

    public Task AddAuditLogAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
        => dbContext.AuditLogs.AddAsync(auditLog, cancellationToken).AsTask();

    public void SetOriginalRowVersion(Employee employee, byte[] rowVersion)
    {
        dbContext.Entry(employee).Property(item => item.RowVersion).OriginalValue = rowVersion;
    }

    public void SetOriginalRowVersion(LeaveRequest leaveRequest, byte[] rowVersion)
    {
        dbContext.Entry(leaveRequest).Property(item => item.RowVersion).OriginalValue = rowVersion;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            throw new ConcurrencyConflictException("The HR data was modified by another user. Refresh the data and try again.", exception);
        }
    }

    private IQueryable<Employee> BuildEmployeeQuery(bool asNoTracking)
    {
        var query = dbContext.Employees
            .Include(item => item.EmploymentContracts)
                .ThenInclude(item => item.Department)
            .Include(item => item.EmploymentContracts)
                .ThenInclude(item => item.Position)
            .Include(item => item.WorkSchedules)
            .Include(item => item.Documents)
            .Include(item => item.DriverProfile)
            .Include(item => item.LeaveRequests)
                .ThenInclude(item => item.LeaveType)
            .AsQueryable();

        return asNoTracking ? query.AsNoTracking() : query;
    }

    private IQueryable<LeaveRequest> BuildLeaveRequestQuery(bool asNoTracking)
    {
        var query = dbContext.LeaveRequests
            .Include(item => item.Employee)
            .Include(item => item.LeaveType)
            .AsQueryable();

        return asNoTracking ? query.AsNoTracking() : query;
    }
}
