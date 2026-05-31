namespace ErpSystem.Domain.Modules.Finance;

public sealed class InvoicePosition : Common.AuditableEntity
{
    private InvoicePosition()
    {
    }

    public InvoicePosition(
        int lineNumber,
        string itemName,
        string description,
        decimal quantity,
        decimal unitPrice,
        decimal taxRate,
        string? notes)
    {
        LineNumber = lineNumber;
        ItemName = itemName;
        Description = description;
        Quantity = quantity;
        UnitPrice = unitPrice;
        TaxRate = taxRate;
        Notes = notes;
    }

    public int InvoiceId { get; private set; }
    public Invoice Invoice { get; private set; } = null!;
    public int LineNumber { get; private set; }
    public string ItemName { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TaxRate { get; private set; }
    public string? Notes { get; private set; }

    public decimal NetAmount => decimal.Round(Quantity * UnitPrice, 2, MidpointRounding.AwayFromZero);
    public decimal TaxAmount => decimal.Round(NetAmount * TaxRate / 100m, 2, MidpointRounding.AwayFromZero);
    public decimal GrossAmount => NetAmount + TaxAmount;

    internal void AssignTo(Invoice invoice)
    {
        Invoice = invoice;
        InvoiceId = invoice.Id;
    }
}
