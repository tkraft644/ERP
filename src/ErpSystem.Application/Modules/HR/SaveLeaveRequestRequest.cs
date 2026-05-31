namespace ErpSystem.Application.Modules.HR;

public sealed record SaveLeaveRequestRequest(
    int EmployeeId,
    int LeaveTypeId,
    DateTime DateFrom,
    DateTime DateTo,
    decimal DayCount,
    string? Reason);

public sealed record DecideLeaveRequestRequest(
    string Status,
    string? DecisionNote,
    byte[] RowVersion);
