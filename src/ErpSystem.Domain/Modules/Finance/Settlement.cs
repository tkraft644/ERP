namespace ErpSystem.Domain.Modules.Finance;

public sealed class Settlement : Common.AuditableEntity
{
    private Settlement()
    {
    }

    public Settlement(int paymentId, int? invoiceId, int? costDocumentId, decimal amount, DateTime settledAtUtc, string? notes)
    {
        PaymentId = paymentId;
        InvoiceId = invoiceId;
        CostDocumentId = costDocumentId;
        Amount = amount;
        SettledAtUtc = settledAtUtc;
        Notes = notes;
    }

    public int PaymentId { get; private set; }
    public Payment Payment { get; private set; } = null!;
    public int? InvoiceId { get; private set; }
    public Invoice? Invoice { get; private set; }
    public int? CostDocumentId { get; private set; }
    public CostDocument? CostDocument { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime SettledAtUtc { get; private set; }
    public string? Notes { get; private set; }
}
