namespace ErpSystem.Application.Modules.Finance;

public sealed record SaveCurrencyRateRequest(
    DateTime RateDate,
    string BaseCurrencyCode,
    string QuoteCurrencyCode,
    decimal Rate,
    string Source,
    byte[]? RowVersion = null);
