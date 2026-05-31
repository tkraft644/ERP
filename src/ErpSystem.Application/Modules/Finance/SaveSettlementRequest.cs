namespace ErpSystem.Application.Modules.Finance;

public sealed record SaveSettlementRequest(
    int PaymentId,
    int? InvoiceId,
    int? CostDocumentId,
    decimal Amount,
    string? Notes);
