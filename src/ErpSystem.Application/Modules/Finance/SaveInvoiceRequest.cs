namespace ErpSystem.Application.Modules.Finance;

public sealed record SaveInvoiceRequest(
    DateTime InvoiceDate,
    DateTime SaleDate,
    DateTime DueDate,
    int ContractorId,
    int? TransportOrderId,
    string CurrencyCode,
    decimal? ExchangeRate,
    string? ExternalNumber,
    string? Description,
    IReadOnlyList<SaveInvoicePositionRequest> Positions,
    byte[]? RowVersion = null);

public sealed record SaveInvoicePositionRequest(
    int LineNumber,
    string ItemName,
    string Description,
    decimal Quantity,
    decimal UnitPrice,
    decimal TaxRate,
    string? Notes);
