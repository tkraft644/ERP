namespace ErpSystem.Domain.Modules.HR;

public sealed class LeaveRequest : Common.AuditableEntity
{
    private LeaveRequest()
    {
    }

    public LeaveRequest(
        int employeeId,
        int leaveTypeId,
        DateTime dateFrom,
        DateTime dateTo,
        decimal dayCount,
        string? reason)
    {
        EmployeeId = employeeId;
        LeaveTypeId = leaveTypeId;
        DateFrom = dateFrom;
        DateTo = dateTo;
        DayCount = dayCount;
        Reason = reason;
        Status = LeaveRequestStatus.Submitted;
    }

    public int EmployeeId { get; private set; }
    public Employee Employee { get; private set; } = null!;
    public int LeaveTypeId { get; private set; }
    public LeaveType LeaveType { get; private set; } = null!;
    public LeaveRequestStatus Status { get; private set; }
    public DateTime DateFrom { get; private set; }
    public DateTime DateTo { get; private set; }
    public decimal DayCount { get; private set; }
    public string? Reason { get; private set; }
    public string? DecisionNote { get; private set; }
    public int? ApprovedByUserId { get; private set; }
    public DateTime? DecidedAtUtc { get; private set; }

    public void Decide(LeaveRequestStatus status, int? approvedByUserId, DateTime decidedAtUtc, string? decisionNote)
    {
        Status = status;
        ApprovedByUserId = approvedByUserId;
        DecidedAtUtc = decidedAtUtc;
        DecisionNote = decisionNote;
    }
}
