namespace ErpSystem.Application.Modules.Finance;

public sealed record SavePaymentRequest(
    DateTime PaymentDate,
    string Direction,
    int? ContractorId,
    string CurrencyCode,
    decimal? ExchangeRate,
    decimal Amount,
    string Method,
    string? ReferenceNumber,
    string? Notes);
