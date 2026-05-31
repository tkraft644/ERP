namespace ErpSystem.Application.Modules.Finance;

public interface IFinanceService
{
    Task<FinanceReferenceDataView> GetReferenceDataAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CostDocumentListItemView>> GetCostDocumentsAsync(CancellationToken cancellationToken = default);
    Task<CostDocumentDetailsView?> GetCostDocumentAsync(int costDocumentId, CancellationToken cancellationToken = default);
    Task<CostDocumentDetailsView> CreateCostDocumentAsync(SaveCostDocumentRequest request, CancellationToken cancellationToken = default);
    Task<CostDocumentDetailsView?> UpdateCostDocumentAsync(int costDocumentId, SaveCostDocumentRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InvoiceListItemView>> GetInvoicesAsync(CancellationToken cancellationToken = default);
    Task<InvoiceDetailsView?> GetInvoiceAsync(int invoiceId, CancellationToken cancellationToken = default);
    Task<InvoiceDetailsView> CreateInvoiceAsync(SaveInvoiceRequest request, CancellationToken cancellationToken = default);
    Task<InvoiceDetailsView?> UpdateInvoiceAsync(int invoiceId, SaveInvoiceRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PaymentListItemView>> GetPaymentsAsync(CancellationToken cancellationToken = default);
    Task<PaymentListItemView> CreatePaymentAsync(SavePaymentRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CurrencyRateView>> GetCurrencyRatesAsync(CancellationToken cancellationToken = default);
    Task<CurrencyRateView> CreateCurrencyRateAsync(SaveCurrencyRateRequest request, CancellationToken cancellationToken = default);
    Task<CurrencyRateView?> UpdateCurrencyRateAsync(int currencyRateId, SaveCurrencyRateRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SettlementView>> GetSettlementsAsync(CancellationToken cancellationToken = default);
    Task<SettlementView> CreateSettlementAsync(SaveSettlementRequest request, CancellationToken cancellationToken = default);
}
