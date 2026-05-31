namespace ErpSystem.Desktop.Services;

public sealed class FinanceWorkspaceStore
{
    private readonly List<FinanceCostDocumentRecord> costDocuments = [];
    private readonly List<FinanceInvoiceRecord> invoices = [];
    private readonly List<FinancePaymentRecord> payments = [];
    private readonly List<FinanceSettlementRecord> settlements = [];
    private readonly List<FinanceCurrencyRateRecord> currencyRates = [];
    private int nextCostDocumentId = 40;
    private int nextInvoiceId = 70;
    private int nextPaymentId = 100;
    private int nextSettlementId = 200;
    private int nextCurrencyRateId = 300;

    private FinanceWorkspaceStore()
    {
        Seed();
    }

    public static FinanceWorkspaceStore Instance { get; } = new();

    public event EventHandler? Changed;

    public IReadOnlyList<FinanceCostDocumentRecord> GetCostDocuments()
        => costDocuments.OrderByDescending(item => item.DocumentDate).ThenByDescending(item => item.Id).ToArray();

    public IReadOnlyList<FinanceInvoiceRecord> GetInvoices()
        => invoices.OrderByDescending(item => item.InvoiceDate).ThenByDescending(item => item.Id).ToArray();

    public IReadOnlyList<FinancePaymentRecord> GetPayments()
        => payments.OrderByDescending(item => item.PaymentDate).ThenByDescending(item => item.Id).ToArray();

    public IReadOnlyList<FinanceSettlementRecord> GetSettlements()
        => settlements.OrderByDescending(item => item.SettledAt).ThenByDescending(item => item.Id).ToArray();

    public IReadOnlyList<FinanceCurrencyRateRecord> GetCurrencyRates()
        => currencyRates.OrderByDescending(item => item.RateDate).ThenBy(item => item.BaseCurrency).ThenBy(item => item.QuoteCurrency).ToArray();

    public IReadOnlyList<string> GetCurrencies()
        => costDocuments.Select(item => item.CurrencyCode)
            .Concat(invoices.Select(item => item.CurrencyCode))
            .Concat(payments.Select(item => item.CurrencyCode))
            .Concat(currencyRates.Select(item => item.BaseCurrency))
            .Concat(currencyRates.Select(item => item.QuoteCurrency))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(item => item)
            .ToArray();

    public IReadOnlyList<string> GetContractors()
        => costDocuments.Select(item => item.ContractorName)
            .Concat(invoices.Select(item => item.ContractorName))
            .Concat(payments.Where(item => !string.IsNullOrWhiteSpace(item.ContractorName)).Select(item => item.ContractorName!))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(item => item)
            .ToArray();

    public FinanceCostDocumentRecord CreateCostDocument(
        string number,
        string status,
        DateTime documentDate,
        DateTime dueDate,
        string contractorName,
        string currencyCode,
        decimal grossAmount,
        string costTarget,
        string description)
    {
        ValidateNumber(number, costDocuments.Select(item => (item.Id, item.Number)), null, "kosztowego");
        var created = new FinanceCostDocumentRecord(
            nextCostDocumentId++,
            number.Trim().ToUpperInvariant(),
            NormalizeStatus(status, "Nowy"),
            documentDate,
            dueDate,
            contractorName.Trim(),
            currencyCode.Trim().ToUpperInvariant(),
            grossAmount,
            costTarget.Trim(),
            NormalizeOptional(description),
            false);
        costDocuments.Add(created);
        RaiseChanged();
        return created;
    }

    public FinanceCostDocumentRecord UpdateCostDocument(
        int id,
        string number,
        string status,
        DateTime documentDate,
        DateTime dueDate,
        string contractorName,
        string currencyCode,
        decimal grossAmount,
        string costTarget,
        string description)
    {
        var current = costDocuments.FirstOrDefault(item => item.Id == id)
            ?? throw new InvalidOperationException("Nie znaleziono dokumentu kosztowego.");
        ValidateNumber(number, costDocuments.Select(item => (item.Id, item.Number)), id, "kosztowego");
        var updated = current with
        {
            Number = number.Trim().ToUpperInvariant(),
            Status = NormalizeStatus(status, "Nowy"),
            DocumentDate = documentDate,
            DueDate = dueDate,
            ContractorName = contractorName.Trim(),
            CurrencyCode = currencyCode.Trim().ToUpperInvariant(),
            GrossAmount = grossAmount,
            CostTarget = costTarget.Trim(),
            Description = NormalizeOptional(description)
        };
        ReplaceCostDocument(updated);
        RaiseChanged();
        return updated;
    }

    public void ArchiveCostDocument(int id)
    {
        var current = costDocuments.FirstOrDefault(item => item.Id == id)
            ?? throw new InvalidOperationException("Nie znaleziono dokumentu kosztowego.");
        ReplaceCostDocument(current with { IsArchived = !current.IsArchived });
        RaiseChanged();
    }

    public void DeleteCostDocument(int id)
    {
        if (costDocuments.RemoveAll(item => item.Id == id) == 0)
        {
            throw new InvalidOperationException("Nie znaleziono dokumentu kosztowego.");
        }
        RaiseChanged();
    }

    public FinanceInvoiceRecord CreateInvoice(
        string number,
        string status,
        DateTime invoiceDate,
        DateTime dueDate,
        string contractorName,
        string currencyCode,
        decimal grossAmount,
        decimal outstandingAmount,
        string transportOrderNumber,
        string description)
    {
        ValidateNumber(number, invoices.Select(item => (item.Id, item.Number)), null, "faktury");
        var created = new FinanceInvoiceRecord(
            nextInvoiceId++,
            number.Trim().ToUpperInvariant(),
            NormalizeStatus(status, "Nowa"),
            invoiceDate,
            dueDate,
            contractorName.Trim(),
            currencyCode.Trim().ToUpperInvariant(),
            grossAmount,
            outstandingAmount,
            NormalizeOptional(transportOrderNumber),
            NormalizeOptional(description),
            false);
        invoices.Add(created);
        RaiseChanged();
        return created;
    }

    public FinanceInvoiceRecord UpdateInvoice(
        int id,
        string number,
        string status,
        DateTime invoiceDate,
        DateTime dueDate,
        string contractorName,
        string currencyCode,
        decimal grossAmount,
        decimal outstandingAmount,
        string transportOrderNumber,
        string description)
    {
        var current = invoices.FirstOrDefault(item => item.Id == id)
            ?? throw new InvalidOperationException("Nie znaleziono faktury.");
        ValidateNumber(number, invoices.Select(item => (item.Id, item.Number)), id, "faktury");
        var updated = current with
        {
            Number = number.Trim().ToUpperInvariant(),
            Status = NormalizeStatus(status, "Nowa"),
            InvoiceDate = invoiceDate,
            DueDate = dueDate,
            ContractorName = contractorName.Trim(),
            CurrencyCode = currencyCode.Trim().ToUpperInvariant(),
            GrossAmount = grossAmount,
            OutstandingAmount = outstandingAmount,
            TransportOrderNumber = NormalizeOptional(transportOrderNumber),
            Description = NormalizeOptional(description)
        };
        ReplaceInvoice(updated);
        RaiseChanged();
        return updated;
    }

    public void ArchiveInvoice(int id)
    {
        var current = invoices.FirstOrDefault(item => item.Id == id)
            ?? throw new InvalidOperationException("Nie znaleziono faktury.");
        ReplaceInvoice(current with { IsArchived = !current.IsArchived });
        RaiseChanged();
    }

    public void DeleteInvoice(int id)
    {
        if (invoices.RemoveAll(item => item.Id == id) == 0)
        {
            throw new InvalidOperationException("Nie znaleziono faktury.");
        }
        RaiseChanged();
    }

    public FinancePaymentRecord CreatePayment(
        DateTime paymentDate,
        string direction,
        string contractorName,
        string currencyCode,
        decimal amount,
        decimal settledAmount,
        string method,
        string referenceNumber,
        string notes)
    {
        var created = new FinancePaymentRecord(
            nextPaymentId++,
            paymentDate,
            NormalizeStatus(direction, "Wpływ"),
            NormalizeOptional(contractorName),
            currencyCode.Trim().ToUpperInvariant(),
            amount,
            settledAmount,
            method.Trim(),
            NormalizeOptional(referenceNumber),
            NormalizeOptional(notes),
            false);
        payments.Add(created);
        RaiseChanged();
        return created;
    }

    public FinancePaymentRecord UpdatePayment(
        int id,
        DateTime paymentDate,
        string direction,
        string contractorName,
        string currencyCode,
        decimal amount,
        decimal settledAmount,
        string method,
        string referenceNumber,
        string notes)
    {
        var current = payments.FirstOrDefault(item => item.Id == id)
            ?? throw new InvalidOperationException("Nie znaleziono płatności.");
        var updated = current with
        {
            PaymentDate = paymentDate,
            Direction = NormalizeStatus(direction, "Wpływ"),
            ContractorName = NormalizeOptional(contractorName),
            CurrencyCode = currencyCode.Trim().ToUpperInvariant(),
            Amount = amount,
            SettledAmount = settledAmount,
            Method = method.Trim(),
            ReferenceNumber = NormalizeOptional(referenceNumber),
            Notes = NormalizeOptional(notes)
        };
        ReplacePayment(updated);
        RaiseChanged();
        return updated;
    }

    public void DeletePayment(int id)
    {
        if (payments.RemoveAll(item => item.Id == id) == 0)
        {
            throw new InvalidOperationException("Nie znaleziono płatności.");
        }
        settlements.RemoveAll(item => item.PaymentReferenceId == id);
        RaiseChanged();
    }

    public FinanceSettlementRecord CreateSettlement(
        int paymentReferenceId,
        string paymentReference,
        string targetDocumentNumber,
        string targetDocumentType,
        decimal amount,
        string status,
        string notes)
    {
        var created = new FinanceSettlementRecord(
            nextSettlementId++,
            paymentReferenceId,
            paymentReference.Trim(),
            targetDocumentNumber.Trim().ToUpperInvariant(),
            targetDocumentType.Trim(),
            amount,
            NormalizeStatus(status, "Nowe"),
            NormalizeOptional(notes),
            DateTime.Now,
            false);
        settlements.Add(created);
        RaiseChanged();
        return created;
    }

    public FinanceSettlementRecord UpdateSettlement(
        int id,
        int paymentReferenceId,
        string paymentReference,
        string targetDocumentNumber,
        string targetDocumentType,
        decimal amount,
        string status,
        string notes)
    {
        var current = settlements.FirstOrDefault(item => item.Id == id)
            ?? throw new InvalidOperationException("Nie znaleziono rozliczenia.");
        var updated = current with
        {
            PaymentReferenceId = paymentReferenceId,
            PaymentReference = paymentReference.Trim(),
            TargetDocumentNumber = targetDocumentNumber.Trim().ToUpperInvariant(),
            TargetDocumentType = targetDocumentType.Trim(),
            Amount = amount,
            Status = NormalizeStatus(status, "Nowe"),
            Notes = NormalizeOptional(notes)
        };
        ReplaceSettlement(updated);
        RaiseChanged();
        return updated;
    }

    public void DeleteSettlement(int id)
    {
        if (settlements.RemoveAll(item => item.Id == id) == 0)
        {
            throw new InvalidOperationException("Nie znaleziono rozliczenia.");
        }
        RaiseChanged();
    }

    public FinanceCurrencyRateRecord CreateCurrencyRate(
        DateTime rateDate,
        string baseCurrency,
        string quoteCurrency,
        decimal rate,
        string source,
        string notes)
    {
        var created = new FinanceCurrencyRateRecord(
            nextCurrencyRateId++,
            rateDate,
            baseCurrency.Trim().ToUpperInvariant(),
            quoteCurrency.Trim().ToUpperInvariant(),
            rate,
            source.Trim(),
            NormalizeOptional(notes),
            false);
        currencyRates.Add(created);
        RaiseChanged();
        return created;
    }

    public FinanceCurrencyRateRecord UpdateCurrencyRate(
        int id,
        DateTime rateDate,
        string baseCurrency,
        string quoteCurrency,
        decimal rate,
        string source,
        string notes)
    {
        var current = currencyRates.FirstOrDefault(item => item.Id == id)
            ?? throw new InvalidOperationException("Nie znaleziono kursu waluty.");
        var updated = current with
        {
            RateDate = rateDate,
            BaseCurrency = baseCurrency.Trim().ToUpperInvariant(),
            QuoteCurrency = quoteCurrency.Trim().ToUpperInvariant(),
            Rate = rate,
            Source = source.Trim(),
            Notes = NormalizeOptional(notes)
        };
        ReplaceCurrencyRate(updated);
        RaiseChanged();
        return updated;
    }

    public void ArchiveCurrencyRate(int id)
    {
        var current = currencyRates.FirstOrDefault(item => item.Id == id)
            ?? throw new InvalidOperationException("Nie znaleziono kursu waluty.");
        ReplaceCurrencyRate(current with { IsArchived = !current.IsArchived });
        RaiseChanged();
    }

    public void DeleteCurrencyRate(int id)
    {
        if (currencyRates.RemoveAll(item => item.Id == id) == 0)
        {
            throw new InvalidOperationException("Nie znaleziono kursu waluty.");
        }
        RaiseChanged();
    }

    private static string? NormalizeOptional(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string NormalizeStatus(string? value, string fallback)
        => string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();

    private static void ValidateNumber(
        string number,
        IEnumerable<(int Id, string Number)> items,
        int? currentId,
        string documentKind)
    {
        if (string.IsNullOrWhiteSpace(number))
        {
            throw new InvalidOperationException($"Numer dokumentu {documentKind} jest wymagany.");
        }

        if (items.Any(item =>
                item.Id != currentId &&
                string.Equals(item.Number, number.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Dokument {documentKind} o takim numerze już istnieje.");
        }
    }

    private void ReplaceCostDocument(FinanceCostDocumentRecord updated)
    {
        var index = costDocuments.FindIndex(item => item.Id == updated.Id);
        if (index >= 0)
        {
            costDocuments[index] = updated;
        }
    }

    private void ReplaceInvoice(FinanceInvoiceRecord updated)
    {
        var index = invoices.FindIndex(item => item.Id == updated.Id);
        if (index >= 0)
        {
            invoices[index] = updated;
        }
    }

    private void ReplacePayment(FinancePaymentRecord updated)
    {
        var index = payments.FindIndex(item => item.Id == updated.Id);
        if (index >= 0)
        {
            payments[index] = updated;
        }
    }

    private void ReplaceSettlement(FinanceSettlementRecord updated)
    {
        var index = settlements.FindIndex(item => item.Id == updated.Id);
        if (index >= 0)
        {
            settlements[index] = updated;
        }
    }

    private void ReplaceCurrencyRate(FinanceCurrencyRateRecord updated)
    {
        var index = currencyRates.FindIndex(item => item.Id == updated.Id);
        if (index >= 0)
        {
            currencyRates[index] = updated;
        }
    }

    private void Seed()
    {
        costDocuments.AddRange(
        [
            new FinanceCostDocumentRecord(nextCostDocumentId++, "KOS/2026/0056", "Do akceptacji", new DateTime(2026, 5, 12), new DateTime(2026, 5, 26), "CMR Logistic Group", "EUR", 12600m, "TR-INT-2026-018", "Koszt przewozu i odprawy eksportowej.", false),
            new FinanceCostDocumentRecord(nextCostDocumentId++, "KOS/2026/0058", "W buforze", new DateTime(2026, 5, 13), new DateTime(2026, 5, 20), "Volvo Parts Polska", "PLN", 24800m, "WH-PZ-2026-0048", "Zakup części do przyjęcia magazynowego.", false),
            new FinanceCostDocumentRecord(nextCostDocumentId++, "KOS/2026/0049", "Zarchiwizowany", new DateTime(2026, 4, 28), new DateTime(2026, 5, 7), "ColorMix Service", "PLN", 3560m, "HR-LAK-04", "Zamknięta usługa serwisowa.", true)
        ]);

        invoices.AddRange(
        [
            new FinanceInvoiceRecord(nextInvoiceId++, "FV/2026/0142", "Wysłana", new DateTime(2026, 5, 14), new DateTime(2026, 5, 28), "Serwis Północ", "PLN", 8430m, 8430m, "TR-KRAJ-2026-221", "Faktura za dostawę i usługę serwisową.", false),
            new FinanceInvoiceRecord(nextInvoiceId++, "FV/2026/0135", "Częściowo rozliczona", new DateTime(2026, 5, 10), new DateTime(2026, 5, 24), "CMR Logistic Group", "EUR", 12480m, 2480m, "TR-INT-2026-018", "Usługa transportu międzynarodowego.", false),
            new FinanceInvoiceRecord(nextInvoiceId++, "FV/2026/0098", "Zarchiwizowana", new DateTime(2026, 4, 9), new DateTime(2026, 4, 23), "Volvo Parts Polska", "PLN", 5100m, 0m, null, "Dokument zamknięty.", true)
        ]);

        payments.AddRange(
        [
            new FinancePaymentRecord(nextPaymentId++, new DateTime(2026, 5, 14), "Wpływ", "Serwis Północ", "PLN", 3500m, 3500m, "Przelew", "WB/2026/224", "Rozliczenie częściowe do FV/2026/0135.", false),
            new FinancePaymentRecord(nextPaymentId++, new DateTime(2026, 5, 13), "Wypływ", "Volvo Parts Polska", "PLN", 24800m, 0m, "Przelew", "WB/2026/218", "Płatność za przyjęcie magazynowe.", false),
            new FinancePaymentRecord(nextPaymentId++, new DateTime(2026, 5, 11), "Wpływ", "CMR Logistic Group", "EUR", 10000m, 10000m, "Przelew", "BNK/2026/078", "Wpływ z dokumentu eksportowego.", false)
        ]);

        settlements.AddRange(
        [
            new FinanceSettlementRecord(nextSettlementId++, 100, "WB/2026/224", "FV/2026/0135", "Faktura", 3500m, "Zaksięgowane", "Powiązano wpływ częściowy.", new DateTime(2026, 5, 14, 15, 30, 0), false),
            new FinanceSettlementRecord(nextSettlementId++, 102, "BNK/2026/078", "FV/2026/0135", "Faktura", 10000m, "Zaksięgowane", "Rozliczenie walutowe po kursie eksportowym.", new DateTime(2026, 5, 11, 12, 18, 0), false)
        ]);

        currencyRates.AddRange(
        [
            new FinanceCurrencyRateRecord(nextCurrencyRateId++, new DateTime(2026, 5, 14), "EUR", "PLN", 4.2812m, "NBP", "Tabela A/93/2026", false),
            new FinanceCurrencyRateRecord(nextCurrencyRateId++, new DateTime(2026, 5, 14), "USD", "PLN", 3.9184m, "NBP", "Tabela A/93/2026", false),
            new FinanceCurrencyRateRecord(nextCurrencyRateId++, new DateTime(2026, 5, 10), "GBP", "PLN", 4.9988m, "Manual", "Kurs do rozliczenia niestandardowego.", true)
        ]);
    }

    private void RaiseChanged() => Changed?.Invoke(this, EventArgs.Empty);
}

public sealed record FinanceCostDocumentRecord(
    int Id,
    string Number,
    string Status,
    DateTime DocumentDate,
    DateTime DueDate,
    string ContractorName,
    string CurrencyCode,
    decimal GrossAmount,
    string CostTarget,
    string? Description,
    bool IsArchived);

public sealed record FinanceInvoiceRecord(
    int Id,
    string Number,
    string Status,
    DateTime InvoiceDate,
    DateTime DueDate,
    string ContractorName,
    string CurrencyCode,
    decimal GrossAmount,
    decimal OutstandingAmount,
    string? TransportOrderNumber,
    string? Description,
    bool IsArchived);

public sealed record FinancePaymentRecord(
    int Id,
    DateTime PaymentDate,
    string Direction,
    string? ContractorName,
    string CurrencyCode,
    decimal Amount,
    decimal SettledAmount,
    string Method,
    string? ReferenceNumber,
    string? Notes,
    bool IsArchived);

public sealed record FinanceSettlementRecord(
    int Id,
    int PaymentReferenceId,
    string PaymentReference,
    string TargetDocumentNumber,
    string TargetDocumentType,
    decimal Amount,
    string Status,
    string? Notes,
    DateTime SettledAt,
    bool IsArchived);

public sealed record FinanceCurrencyRateRecord(
    int Id,
    DateTime RateDate,
    string BaseCurrency,
    string QuoteCurrency,
    decimal Rate,
    string Source,
    string? Notes,
    bool IsArchived);
