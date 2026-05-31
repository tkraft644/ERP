namespace ErpSystem.Application.Modules.HR;

public sealed record LeaveRequestListItemView(
    int Id,
    int EmployeeId,
    string EmployeeNumber,
    string EmployeeFullName,
    int LeaveTypeId,
    string LeaveTypeName,
    string Status,
    DateTime DateFrom,
    DateTime DateTo,
    decimal DayCount,
    string? Reason,
    string? DecisionNote,
    DateTime? DecidedAtUtc,
    byte[] RowVersion);
