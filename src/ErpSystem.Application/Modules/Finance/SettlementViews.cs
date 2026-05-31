namespace ErpSystem.Application.Modules.Finance;

public sealed record SettlementView(
    int Id,
    int PaymentId,
    DateTime PaymentDate,
    string PaymentDirection,
    int? InvoiceId,
    string? InvoiceNumber,
    int? CostDocumentId,
    string? CostDocumentNumber,
    decimal Amount,
    DateTime SettledAtUtc,
    string? Notes);
