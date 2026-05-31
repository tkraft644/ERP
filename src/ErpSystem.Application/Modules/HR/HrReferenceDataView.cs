namespace ErpSystem.Application.Modules.HR;

public sealed record HrDepartmentOptionView(int Id, string Code, string Name);

public sealed record HrPositionOptionView(int Id, string Code, string Name);

public sealed record HrLeaveTypeOptionView(int Id, string Code, string Name, bool IsPaid, bool RequiresApproval);

public sealed record HrEmployeeOptionView(int Id, string EmployeeNumber, string FullName, bool HasDriverProfile);

public sealed record HrLeaveStatusOptionView(string Key, string Label);

public sealed record HrContractTypeOptionView(string Key, string Label);

public sealed record HrReferenceDataView(
    IReadOnlyList<HrDepartmentOptionView> Departments,
    IReadOnlyList<HrPositionOptionView> Positions,
    IReadOnlyList<HrLeaveTypeOptionView> LeaveTypes,
    IReadOnlyList<HrEmployeeOptionView> Employees,
    IReadOnlyList<HrLeaveStatusOptionView> LeaveStatuses,
    IReadOnlyList<HrContractTypeOptionView> ContractTypes);
