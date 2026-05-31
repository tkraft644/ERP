namespace ErpSystem.Application.Modules.Finance;

public sealed record FinanceContractorOptionView(int Id, string Code, string Name);

public sealed record FinanceTransportOrderOptionView(int Id, string Number, string OrderType);

public sealed record FinanceVehicleOptionView(int Id, string RegistrationNumber, string DisplayName);

public sealed record FinanceEmployeeOptionView(int Id, string EmployeeNumber, string FullName);

public sealed record FinanceWarehouseOptionView(int Id, string Code, string Name);

public sealed record FinanceDepartmentOptionView(int Id, string Code, string Name);

public sealed record FinanceCurrencyOptionView(string Code, string Name);

public sealed record FinanceStatusOptionView(string Key, string Label);

public sealed record FinancePaymentDirectionOptionView(string Key, string Label);

public sealed record FinanceReferenceDataView(
    IReadOnlyList<FinanceContractorOptionView> Contractors,
    IReadOnlyList<FinanceTransportOrderOptionView> TransportOrders,
    IReadOnlyList<FinanceVehicleOptionView> Vehicles,
    IReadOnlyList<FinanceEmployeeOptionView> Employees,
    IReadOnlyList<FinanceWarehouseOptionView> Warehouses,
    IReadOnlyList<FinanceDepartmentOptionView> Departments,
    IReadOnlyList<FinanceCurrencyOptionView> Currencies,
    IReadOnlyList<FinanceStatusOptionView> CostStatuses,
    IReadOnlyList<FinanceStatusOptionView> InvoiceStatuses,
    IReadOnlyList<FinancePaymentDirectionOptionView> PaymentDirections);
