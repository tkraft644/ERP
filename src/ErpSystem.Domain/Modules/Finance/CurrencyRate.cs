namespace ErpSystem.Domain.Modules.Finance;

public sealed class CurrencyRate : Common.AuditableEntity
{
    private CurrencyRate()
    {
    }

    public CurrencyRate(DateTime rateDate, string baseCurrencyCode, string quoteCurrencyCode, decimal rate, string source)
    {
        RateDate = rateDate.Date;
        BaseCurrencyCode = baseCurrencyCode;
        QuoteCurrencyCode = quoteCurrencyCode;
        Rate = rate;
        Source = source;
    }

    public DateTime RateDate { get; private set; }
    public string BaseCurrencyCode { get; private set; } = string.Empty;
    public string QuoteCurrencyCode { get; private set; } = string.Empty;
    public decimal Rate { get; private set; }
    public string Source { get; private set; } = string.Empty;

    public void Update(DateTime rateDate, string baseCurrencyCode, string quoteCurrencyCode, decimal rate, string source)
    {
        RateDate = rateDate.Date;
        BaseCurrencyCode = baseCurrencyCode;
        QuoteCurrencyCode = quoteCurrencyCode;
        Rate = rate;
        Source = source;
    }
}
