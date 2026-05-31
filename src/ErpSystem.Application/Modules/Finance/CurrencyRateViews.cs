namespace ErpSystem.Application.Modules.Finance;

public sealed record CurrencyRateView(
    int Id,
    DateTime RateDate,
    string BaseCurrencyCode,
    string QuoteCurrencyCode,
    decimal Rate,
    string Source,
    byte[] RowVersion);
