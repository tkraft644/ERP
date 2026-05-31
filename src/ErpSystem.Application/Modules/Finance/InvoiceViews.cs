namespace ErpSystem.Application.Modules.Finance;

public sealed record InvoicePositionView(
    int Id,
    int LineNumber,
    string ItemName,
    string Description,
    decimal Quantity,
    decimal UnitPrice,
    decimal TaxRate,
    decimal NetAmount,
    decimal TaxAmount,
    decimal GrossAmount,
    string? Notes);

public sealed record InvoiceListItemView(
    int Id,
    string Number,
    string Status,
    DateTime InvoiceDate,
    DateTime DueDate,
    string ContractorName,
    string CurrencyCode,
    decimal TotalGrossAmount,
    decimal SettledAmount,
    decimal OutstandingAmount,
    string? TransportOrderNumber,
    byte[] RowVersion);

public sealed record InvoiceDetailsView(
    int Id,
    string Number,
    string Status,
    DateTime InvoiceDate,
    DateTime SaleDate,
    DateTime DueDate,
    int ContractorId,
    string ContractorName,
    int? TransportOrderId,
    string? TransportOrderNumber,
    string CurrencyCode,
    decimal? ExchangeRate,
    string? ExternalNumber,
    string? Description,
    IReadOnlyList<InvoicePositionView> Positions,
    decimal TotalNetAmount,
    decimal TotalGrossAmount,
    decimal SettledAmount,
    decimal OutstandingAmount,
    byte[] RowVersion);
