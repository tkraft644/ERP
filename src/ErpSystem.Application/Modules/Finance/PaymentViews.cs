namespace ErpSystem.Application.Modules.Finance;

public sealed record PaymentListItemView(
    int Id,
    DateTime PaymentDate,
    string Direction,
    int? ContractorId,
    string? ContractorName,
    string CurrencyCode,
    decimal Amount,
    string Method,
    string? ReferenceNumber,
    string? Notes,
    decimal SettledAmount,
    decimal RemainingAmount);
