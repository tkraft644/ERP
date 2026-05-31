using System.Text.Json;
using ErpSystem.Application.Common;
using ErpSystem.Domain.Modules.Finance;
using ErpSystem.Domain.Modules.System.Auditing;

namespace ErpSystem.Application.Modules.Finance;

public sealed class FinanceService : IFinanceService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly IFinanceRepository repository;
    private readonly ICurrentUserAccessor currentUserAccessor;

    public FinanceService(IFinanceRepository repository, ICurrentUserAccessor currentUserAccessor)
    {
        this.repository = repository;
        this.currentUserAccessor = currentUserAccessor;
    }

    public async Task<FinanceReferenceDataView> GetReferenceDataAsync(CancellationToken cancellationToken = default)
    {
        var contractors = await repository.GetContractorsAsync(cancellationToken);
        var transportOrders = await repository.GetTransportOrdersAsync(cancellationToken);
        var vehicles = await repository.GetVehiclesAsync(cancellationToken);
        var employees = await repository.GetEmployeesAsync(cancellationToken);
        var warehouses = await repository.GetWarehousesAsync(cancellationToken);
        var departments = await repository.GetDepartmentsAsync(cancellationToken);
        var currencies = await repository.GetCurrenciesAsync(cancellationToken);

        return new FinanceReferenceDataView(
            contractors.OrderBy(item => item.Name)
                .Select(item => new FinanceContractorOptionView(item.Id, item.Code, item.Name))
                .ToArray(),
            transportOrders.OrderByDescending(item => item.OrderDate)
                .ThenByDescending(item => item.Id)
                .Select(item => new FinanceTransportOrderOptionView(item.Id, item.Number, item.OrderType.ToString()))
                .ToArray(),
            vehicles.OrderBy(item => item.RegistrationNumber)
                .Select(item => new FinanceVehicleOptionView(item.Id, item.RegistrationNumber, $"{item.RegistrationNumber} • {item.Brand} {item.Model}"))
                .ToArray(),
            employees.OrderBy(item => item.LastName).ThenBy(item => item.FirstName)
                .Select(item => new FinanceEmployeeOptionView(item.Id, item.EmployeeNumber, item.FullName))
                .ToArray(),
            warehouses.OrderBy(item => item.Name)
                .Select(item => new FinanceWarehouseOptionView(item.Id, item.Code, item.Name))
                .ToArray(),
            departments.OrderBy(item => item.Name)
                .Select(item => new FinanceDepartmentOptionView(item.Id, item.Code, item.Name))
                .ToArray(),
            currencies.OrderBy(item => item.SortOrder).ThenBy(item => item.Code)
                .Select(item => new FinanceCurrencyOptionView(item.Code, item.Name))
                .ToArray(),
            GetCostStatuses(),
            GetInvoiceStatuses(),
            GetPaymentDirections());
    }

    public async Task<IReadOnlyList<CostDocumentListItemView>> GetCostDocumentsAsync(CancellationToken cancellationToken = default)
    {
        var documents = await repository.GetCostDocumentsAsync(cancellationToken);
        return documents
            .OrderByDescending(item => item.DocumentDate)
            .ThenByDescending(item => item.Id)
            .Select(MapCostDocumentListItem)
            .ToArray();
    }

    public async Task<CostDocumentDetailsView?> GetCostDocumentAsync(int costDocumentId, CancellationToken cancellationToken = default)
    {
        var document = await repository.GetCostDocumentAsync(costDocumentId, cancellationToken);
        return document is null ? null : MapCostDocumentDetails(document);
    }

    public async Task<CostDocumentDetailsView> CreateCostDocumentAsync(SaveCostDocumentRequest request, CancellationToken cancellationToken = default)
    {
        ValidateCostDocumentRequest(request);
        var number = await repository.GenerateDocumentNumberAsync("FIN_COST", DateTime.UtcNow, cancellationToken);
        var document = BuildCostDocument(number, request);

        await repository.AddCostDocumentAsync(document, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        var created = await repository.GetCostDocumentAsync(document.Id, cancellationToken)
            ?? throw new InvalidOperationException("Created cost document could not be reloaded.");
        await repository.AddAuditLogAsync(CreateAuditLog(
            "CostDocument",
            created.Id,
            "Created",
            "{}",
            Serialize(MapCostDocumentDetails(created)),
            $"Created cost document '{created.Number}'."),
            cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return MapCostDocumentDetails(created);
    }

    public async Task<CostDocumentDetailsView?> UpdateCostDocumentAsync(int costDocumentId, SaveCostDocumentRequest request, CancellationToken cancellationToken = default)
    {
        if (request.RowVersion is null || request.RowVersion.Length == 0)
        {
            throw new ArgumentException("RowVersion is required when updating a cost document.", nameof(request));
        }

        ValidateCostDocumentRequest(request);
        var document = await repository.GetCostDocumentForUpdateAsync(costDocumentId, cancellationToken);
        if (document is null)
        {
            return null;
        }

        repository.SetOriginalRowVersion(document, request.RowVersion);
        var previous = MapCostDocumentDetails(document);
        document.UpdateCore(
            request.DocumentDate,
            request.PostingDate,
            request.DueDate,
            request.ContractorId,
            NormalizeRequired(request.CurrencyCode, nameof(request.CurrencyCode)).ToUpperInvariant(),
            request.ExchangeRate,
            NormalizeOptional(request.ExternalNumber),
            NormalizeOptional(request.Description));
        document.ReplacePositions(BuildCostPositions(request.Positions));

        await repository.SaveChangesAsync(cancellationToken);
        var updated = await repository.GetCostDocumentAsync(document.Id, cancellationToken)
            ?? throw new InvalidOperationException("Updated cost document could not be reloaded.");
        await repository.AddAuditLogAsync(CreateAuditLog(
            "CostDocument",
            updated.Id,
            "Updated",
            Serialize(previous),
            Serialize(MapCostDocumentDetails(updated)),
            $"Updated cost document '{updated.Number}'."),
            cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return MapCostDocumentDetails(updated);
    }

    public async Task<IReadOnlyList<InvoiceListItemView>> GetInvoicesAsync(CancellationToken cancellationToken = default)
    {
        var invoices = await repository.GetInvoicesAsync(cancellationToken);
        return invoices
            .OrderByDescending(item => item.InvoiceDate)
            .ThenByDescending(item => item.Id)
            .Select(MapInvoiceListItem)
            .ToArray();
    }

    public async Task<InvoiceDetailsView?> GetInvoiceAsync(int invoiceId, CancellationToken cancellationToken = default)
    {
        var invoice = await repository.GetInvoiceAsync(invoiceId, cancellationToken);
        return invoice is null ? null : MapInvoiceDetails(invoice);
    }

    public async Task<InvoiceDetailsView> CreateInvoiceAsync(SaveInvoiceRequest request, CancellationToken cancellationToken = default)
    {
        ValidateInvoiceRequest(request);
        var number = await repository.GenerateDocumentNumberAsync("FIN_INV", DateTime.UtcNow, cancellationToken);
        var invoice = BuildInvoice(number, request);

        await repository.AddInvoiceAsync(invoice, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        var created = await repository.GetInvoiceAsync(invoice.Id, cancellationToken)
            ?? throw new InvalidOperationException("Created invoice could not be reloaded.");
        await repository.AddAuditLogAsync(CreateAuditLog(
            "Invoice",
            created.Id,
            "Created",
            "{}",
            Serialize(MapInvoiceDetails(created)),
            $"Created invoice '{created.Number}'."),
            cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return MapInvoiceDetails(created);
    }

    public async Task<InvoiceDetailsView?> UpdateInvoiceAsync(int invoiceId, SaveInvoiceRequest request, CancellationToken cancellationToken = default)
    {
        if (request.RowVersion is null || request.RowVersion.Length == 0)
        {
            throw new ArgumentException("RowVersion is required when updating an invoice.", nameof(request));
        }

        ValidateInvoiceRequest(request);
        var invoice = await repository.GetInvoiceForUpdateAsync(invoiceId, cancellationToken);
        if (invoice is null)
        {
            return null;
        }

        repository.SetOriginalRowVersion(invoice, request.RowVersion);
        var previous = MapInvoiceDetails(invoice);
        invoice.UpdateCore(
            request.InvoiceDate,
            request.SaleDate,
            request.DueDate,
            request.ContractorId,
            request.TransportOrderId,
            NormalizeRequired(request.CurrencyCode, nameof(request.CurrencyCode)).ToUpperInvariant(),
            request.ExchangeRate,
            NormalizeOptional(request.ExternalNumber),
            NormalizeOptional(request.Description));
        invoice.ReplacePositions(BuildInvoicePositions(request.Positions));
        invoice.RecalculateStatus(invoice.Settlements.Sum(item => item.Amount));

        await repository.SaveChangesAsync(cancellationToken);
        var updated = await repository.GetInvoiceAsync(invoice.Id, cancellationToken)
            ?? throw new InvalidOperationException("Updated invoice could not be reloaded.");
        await repository.AddAuditLogAsync(CreateAuditLog(
            "Invoice",
            updated.Id,
            "Updated",
            Serialize(previous),
            Serialize(MapInvoiceDetails(updated)),
            $"Updated invoice '{updated.Number}'."),
            cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return MapInvoiceDetails(updated);
    }

    public async Task<IReadOnlyList<PaymentListItemView>> GetPaymentsAsync(CancellationToken cancellationToken = default)
    {
        var payments = await repository.GetPaymentsAsync(cancellationToken);
        return payments
            .OrderByDescending(item => item.PaymentDate)
            .ThenByDescending(item => item.Id)
            .Select(MapPayment)
            .ToArray();
    }

    public async Task<PaymentListItemView> CreatePaymentAsync(SavePaymentRequest request, CancellationToken cancellationToken = default)
    {
        ValidatePaymentRequest(request);
        var payment = new Payment(
            request.PaymentDate,
            ParsePaymentDirection(request.Direction),
            request.ContractorId,
            NormalizeRequired(request.CurrencyCode, nameof(request.CurrencyCode)).ToUpperInvariant(),
            request.ExchangeRate,
            request.Amount,
            NormalizeRequired(request.Method, nameof(request.Method)),
            NormalizeOptional(request.ReferenceNumber),
            NormalizeOptional(request.Notes));
        await repository.AddPaymentAsync(payment, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        var created = (await repository.GetPaymentsAsync(cancellationToken)).Single(item => item.Id == payment.Id);
        await repository.AddAuditLogAsync(CreateAuditLog(
            "Payment",
            created.Id,
            "Created",
            "{}",
            Serialize(MapPayment(created)),
            $"Created payment '{created.Id}'."),
            cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return MapPayment(created);
    }

    public async Task<IReadOnlyList<CurrencyRateView>> GetCurrencyRatesAsync(CancellationToken cancellationToken = default)
    {
        var rates = await repository.GetCurrencyRatesAsync(cancellationToken);
        return rates
            .OrderByDescending(item => item.RateDate)
            .ThenBy(item => item.BaseCurrencyCode)
            .ThenBy(item => item.QuoteCurrencyCode)
            .Select(MapCurrencyRate)
            .ToArray();
    }

    public async Task<CurrencyRateView> CreateCurrencyRateAsync(SaveCurrencyRateRequest request, CancellationToken cancellationToken = default)
    {
        ValidateCurrencyRateRequest(request);
        var rate = new CurrencyRate(
            request.RateDate,
            NormalizeRequired(request.BaseCurrencyCode, nameof(request.BaseCurrencyCode)).ToUpperInvariant(),
            NormalizeRequired(request.QuoteCurrencyCode, nameof(request.QuoteCurrencyCode)).ToUpperInvariant(),
            request.Rate,
            NormalizeRequired(request.Source, nameof(request.Source)));
        await repository.AddCurrencyRateAsync(rate, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return MapCurrencyRate(rate);
    }

    public async Task<CurrencyRateView?> UpdateCurrencyRateAsync(int currencyRateId, SaveCurrencyRateRequest request, CancellationToken cancellationToken = default)
    {
        if (request.RowVersion is null || request.RowVersion.Length == 0)
        {
            throw new ArgumentException("RowVersion is required when updating a currency rate.", nameof(request));
        }

        ValidateCurrencyRateRequest(request);
        var rate = await repository.GetCurrencyRateForUpdateAsync(currencyRateId, cancellationToken);
        if (rate is null)
        {
            return null;
        }

        repository.SetOriginalRowVersion(rate, request.RowVersion);
        rate.Update(
            request.RateDate,
            NormalizeRequired(request.BaseCurrencyCode, nameof(request.BaseCurrencyCode)).ToUpperInvariant(),
            NormalizeRequired(request.QuoteCurrencyCode, nameof(request.QuoteCurrencyCode)).ToUpperInvariant(),
            request.Rate,
            NormalizeRequired(request.Source, nameof(request.Source)));
        await repository.SaveChangesAsync(cancellationToken);
        return MapCurrencyRate(rate);
    }

    public async Task<IReadOnlyList<SettlementView>> GetSettlementsAsync(CancellationToken cancellationToken = default)
    {
        var settlements = await repository.GetSettlementsAsync(cancellationToken);
        return settlements
            .OrderByDescending(item => item.SettledAtUtc)
            .ThenByDescending(item => item.Id)
            .Select(MapSettlement)
            .ToArray();
    }

    public async Task<SettlementView> CreateSettlementAsync(SaveSettlementRequest request, CancellationToken cancellationToken = default)
    {
        ValidateSettlementRequest(request);
        var payment = await repository.GetPaymentForSettlementAsync(request.PaymentId, cancellationToken)
            ?? throw new InvalidOperationException("Payment for settlement was not found.");

        var paymentRemaining = payment.Amount - payment.Settlements.Sum(item => item.Amount);
        if (request.Amount > paymentRemaining)
        {
            throw new InvalidOperationException("Settlement amount cannot exceed remaining payment amount.");
        }

        Invoice? invoice = null;
        if (request.InvoiceId.HasValue)
        {
            invoice = await repository.GetInvoiceForUpdateAsync(request.InvoiceId.Value, cancellationToken)
                ?? throw new InvalidOperationException("Invoice for settlement was not found.");
            var invoiceRemaining = invoice.GetGrossTotal() - invoice.Settlements.Sum(item => item.Amount);
            if (request.Amount > invoiceRemaining)
            {
                throw new InvalidOperationException("Settlement amount cannot exceed outstanding invoice amount.");
            }
        }

        CostDocument? costDocument = null;
        if (request.CostDocumentId.HasValue)
        {
            costDocument = await repository.GetCostDocumentForUpdateAsync(request.CostDocumentId.Value, cancellationToken)
                ?? throw new InvalidOperationException("Cost document for settlement was not found.");
            var costRemaining = costDocument.Positions.Sum(item => item.GrossAmount) - costDocument.Settlements.Sum(item => item.Amount);
            if (request.Amount > costRemaining)
            {
                throw new InvalidOperationException("Settlement amount cannot exceed remaining cost document amount.");
            }
        }

        var settlement = new Settlement(
            request.PaymentId,
            request.InvoiceId,
            request.CostDocumentId,
            request.Amount,
            DateTime.UtcNow,
            NormalizeOptional(request.Notes));
        await repository.AddSettlementAsync(settlement, cancellationToken);

        if (invoice is not null)
        {
            invoice.RecalculateStatus(invoice.Settlements.Sum(item => item.Amount) + request.Amount);
        }

        await repository.SaveChangesAsync(cancellationToken);
        var created = (await repository.GetSettlementsAsync(cancellationToken)).Single(item => item.Id == settlement.Id);
        await repository.AddAuditLogAsync(CreateAuditLog(
            "Settlement",
            created.Id,
            "Created",
            "{}",
            Serialize(MapSettlement(created)),
            $"Created settlement '{created.Id}'."),
            cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return MapSettlement(created);
    }

    private static CostDocument BuildCostDocument(string number, SaveCostDocumentRequest request)
    {
        var document = new CostDocument(
            number,
            request.DocumentDate,
            request.PostingDate,
            request.DueDate,
            request.ContractorId,
            NormalizeRequired(request.CurrencyCode, nameof(request.CurrencyCode)).ToUpperInvariant(),
            request.ExchangeRate,
            NormalizeOptional(request.ExternalNumber),
            NormalizeOptional(request.Description));
        document.ReplacePositions(BuildCostPositions(request.Positions));
        return document;
    }

    private static CostDocumentListItemView MapCostDocumentListItem(CostDocument document)
    {
        var totalNet = document.Positions.Sum(item => item.NetAmount);
        var totalGross = document.Positions.Sum(item => item.GrossAmount);
        return new CostDocumentListItemView(
            document.Id,
            document.Number,
            document.Status.ToString(),
            document.DocumentDate,
            document.PostingDate,
            document.DueDate,
            document.Contractor?.Name,
            document.CurrencyCode,
            totalNet,
            totalGross,
            document.RowVersion);
    }

    private static CostDocumentDetailsView MapCostDocumentDetails(CostDocument document)
    {
        var totalNet = document.Positions.Sum(item => item.NetAmount);
        var totalGross = document.Positions.Sum(item => item.GrossAmount);
        return new CostDocumentDetailsView(
            document.Id,
            document.Number,
            document.Status.ToString(),
            document.DocumentDate,
            document.PostingDate,
            document.DueDate,
            document.ContractorId,
            document.Contractor?.Name,
            document.CurrencyCode,
            document.ExchangeRate,
            document.ExternalNumber,
            document.Description,
            document.Positions.OrderBy(item => item.LineNumber)
                .Select(MapCostPosition)
                .ToArray(),
            totalNet,
            totalGross,
            document.RowVersion);
    }

    private static CostPositionView MapCostPosition(CostPosition position)
        => new(
            position.Id,
            position.LineNumber,
            position.CostCategory,
            position.Description,
            position.Quantity,
            position.UnitPrice,
            position.TaxRate,
            position.NetAmount,
            position.TaxAmount,
            position.GrossAmount,
            position.TransportOrderId,
            position.TransportOrder?.Number,
            position.VehicleId,
            position.Vehicle?.RegistrationNumber,
            position.EmployeeId,
            position.Employee?.FullName,
            position.WarehouseId,
            position.Warehouse?.Name,
            position.ContractorId,
            position.Contractor?.Name,
            position.DepartmentId,
            position.Department?.Name,
            position.Notes);

    private static Invoice BuildInvoice(string number, SaveInvoiceRequest request)
    {
        var invoice = new Invoice(
            number,
            request.InvoiceDate,
            request.SaleDate,
            request.DueDate,
            request.ContractorId,
            request.TransportOrderId,
            NormalizeRequired(request.CurrencyCode, nameof(request.CurrencyCode)).ToUpperInvariant(),
            request.ExchangeRate,
            NormalizeOptional(request.ExternalNumber),
            NormalizeOptional(request.Description));
        invoice.ReplacePositions(BuildInvoicePositions(request.Positions));
        invoice.RecalculateStatus(0m);
        return invoice;
    }

    private static InvoiceListItemView MapInvoiceListItem(Invoice invoice)
    {
        var totalGross = invoice.Positions.Sum(item => item.GrossAmount);
        var settledAmount = invoice.Settlements.Sum(item => item.Amount);
        return new InvoiceListItemView(
            invoice.Id,
            invoice.Number,
            invoice.Status.ToString(),
            invoice.InvoiceDate,
            invoice.DueDate,
            invoice.Contractor.Name,
            invoice.CurrencyCode,
            totalGross,
            settledAmount,
            totalGross - settledAmount,
            invoice.TransportOrder?.Number,
            invoice.RowVersion);
    }

    private static InvoiceDetailsView MapInvoiceDetails(Invoice invoice)
    {
        var totalNet = invoice.Positions.Sum(item => item.NetAmount);
        var totalGross = invoice.Positions.Sum(item => item.GrossAmount);
        var settledAmount = invoice.Settlements.Sum(item => item.Amount);
        return new InvoiceDetailsView(
            invoice.Id,
            invoice.Number,
            invoice.Status.ToString(),
            invoice.InvoiceDate,
            invoice.SaleDate,
            invoice.DueDate,
            invoice.ContractorId,
            invoice.Contractor.Name,
            invoice.TransportOrderId,
            invoice.TransportOrder?.Number,
            invoice.CurrencyCode,
            invoice.ExchangeRate,
            invoice.ExternalNumber,
            invoice.Description,
            invoice.Positions.OrderBy(item => item.LineNumber)
                .Select(item => new InvoicePositionView(
                    item.Id,
                    item.LineNumber,
                    item.ItemName,
                    item.Description,
                    item.Quantity,
                    item.UnitPrice,
                    item.TaxRate,
                    item.NetAmount,
                    item.TaxAmount,
                    item.GrossAmount,
                    item.Notes))
                .ToArray(),
            totalNet,
            totalGross,
            settledAmount,
            totalGross - settledAmount,
            invoice.RowVersion);
    }

    private static PaymentListItemView MapPayment(Payment payment)
    {
        var settledAmount = payment.Settlements.Sum(item => item.Amount);
        return new PaymentListItemView(
            payment.Id,
            payment.PaymentDate,
            payment.Direction.ToString(),
            payment.ContractorId,
            payment.Contractor?.Name,
            payment.CurrencyCode,
            payment.Amount,
            payment.Method,
            payment.ReferenceNumber,
            payment.Notes,
            settledAmount,
            payment.Amount - settledAmount);
    }

    private static CurrencyRateView MapCurrencyRate(CurrencyRate rate)
        => new(rate.Id, rate.RateDate, rate.BaseCurrencyCode, rate.QuoteCurrencyCode, rate.Rate, rate.Source, rate.RowVersion);

    private static SettlementView MapSettlement(Settlement settlement)
        => new(
            settlement.Id,
            settlement.PaymentId,
            settlement.Payment.PaymentDate,
            settlement.Payment.Direction.ToString(),
            settlement.InvoiceId,
            settlement.Invoice?.Number,
            settlement.CostDocumentId,
            settlement.CostDocument?.Number,
            settlement.Amount,
            settlement.SettledAtUtc,
            settlement.Notes);

    private static IReadOnlyList<CostPosition> BuildCostPositions(IReadOnlyList<SaveCostPositionRequest> requests)
        => requests
            .OrderBy(item => item.LineNumber)
            .Select(item => new CostPosition(
                item.LineNumber,
                NormalizeRequired(item.CostCategory, nameof(item.CostCategory)),
                NormalizeRequired(item.Description, nameof(item.Description)),
                item.Quantity,
                item.UnitPrice,
                item.TaxRate,
                item.TransportOrderId,
                item.VehicleId,
                item.EmployeeId,
                item.WarehouseId,
                item.ContractorId,
                item.DepartmentId,
                NormalizeOptional(item.Notes)))
            .ToArray();

    private static IReadOnlyList<InvoicePosition> BuildInvoicePositions(IReadOnlyList<SaveInvoicePositionRequest> requests)
        => requests
            .OrderBy(item => item.LineNumber)
            .Select(item => new InvoicePosition(
                item.LineNumber,
                NormalizeRequired(item.ItemName, nameof(item.ItemName)),
                NormalizeRequired(item.Description, nameof(item.Description)),
                item.Quantity,
                item.UnitPrice,
                item.TaxRate,
                NormalizeOptional(item.Notes)))
            .ToArray();

    private static void ValidateCostDocumentRequest(SaveCostDocumentRequest request)
    {
        if (request.Positions.Count == 0)
        {
            throw new ArgumentException("Cost document must contain at least one position.", nameof(request.Positions));
        }

        ValidateCurrency(request.CurrencyCode, request.ExchangeRate);
        ValidateCostPositions(request.Positions);
    }

    private static void ValidateInvoiceRequest(SaveInvoiceRequest request)
    {
        if (request.Positions.Count == 0)
        {
            throw new ArgumentException("Invoice must contain at least one position.", nameof(request.Positions));
        }

        ValidateCurrency(request.CurrencyCode, request.ExchangeRate);
        ValidateInvoicePositions(request.Positions);
    }

    private static void ValidatePaymentRequest(SavePaymentRequest request)
    {
        ValidateCurrency(request.CurrencyCode, request.ExchangeRate);
        _ = ParsePaymentDirection(request.Direction);
        _ = NormalizeRequired(request.Method, nameof(request.Method));
        if (request.Amount <= 0m)
        {
            throw new ArgumentException("Payment amount must be greater than zero.", nameof(request.Amount));
        }
    }

    private static void ValidateCurrencyRateRequest(SaveCurrencyRateRequest request)
    {
        if (request.Rate <= 0m)
        {
            throw new ArgumentException("Currency rate must be greater than zero.", nameof(request.Rate));
        }

        var baseCurrency = NormalizeRequired(request.BaseCurrencyCode, nameof(request.BaseCurrencyCode)).ToUpperInvariant();
        var quoteCurrency = NormalizeRequired(request.QuoteCurrencyCode, nameof(request.QuoteCurrencyCode)).ToUpperInvariant();
        if (string.Equals(baseCurrency, quoteCurrency, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Base and quote currency codes must differ.", nameof(request.QuoteCurrencyCode));
        }

        _ = NormalizeRequired(request.Source, nameof(request.Source));
    }

    private static void ValidateSettlementRequest(SaveSettlementRequest request)
    {
        if ((request.InvoiceId.HasValue ? 1 : 0) + (request.CostDocumentId.HasValue ? 1 : 0) != 1)
        {
            throw new ArgumentException("Settlement must point to exactly one target document.", nameof(request));
        }

        if (request.Amount <= 0m)
        {
            throw new ArgumentException("Settlement amount must be greater than zero.", nameof(request.Amount));
        }
    }

    private static void ValidateCurrency(string currencyCode, decimal? exchangeRate)
    {
        var normalized = NormalizeRequired(currencyCode, nameof(currencyCode)).ToUpperInvariant();
        if (!string.Equals(normalized, "PLN", StringComparison.OrdinalIgnoreCase) && (exchangeRate is null or <= 0m))
        {
            throw new ArgumentException("ExchangeRate is required for non-PLN documents.", nameof(exchangeRate));
        }
    }

    private static void ValidateCostPositions(IReadOnlyList<SaveCostPositionRequest> positions)
    {
        if (positions.Select(item => item.LineNumber).Distinct().Count() != positions.Count)
        {
            throw new ArgumentException("Cost position line numbers must be unique.", nameof(positions));
        }

        foreach (var position in positions)
        {
            _ = NormalizeRequired(position.CostCategory, nameof(position.CostCategory));
            _ = NormalizeRequired(position.Description, nameof(position.Description));
            if (position.Quantity <= 0m)
            {
                throw new ArgumentException("Cost position quantity must be greater than zero.", nameof(positions));
            }

            if (position.UnitPrice < 0m || position.TaxRate < 0m)
            {
                throw new ArgumentException("Cost position prices and tax rate cannot be negative.", nameof(positions));
            }
        }
    }

    private static void ValidateInvoicePositions(IReadOnlyList<SaveInvoicePositionRequest> positions)
    {
        if (positions.Select(item => item.LineNumber).Distinct().Count() != positions.Count)
        {
            throw new ArgumentException("Invoice position line numbers must be unique.", nameof(positions));
        }

        foreach (var position in positions)
        {
            _ = NormalizeRequired(position.ItemName, nameof(position.ItemName));
            _ = NormalizeRequired(position.Description, nameof(position.Description));
            if (position.Quantity <= 0m)
            {
                throw new ArgumentException("Invoice position quantity must be greater than zero.", nameof(positions));
            }

            if (position.UnitPrice < 0m || position.TaxRate < 0m)
            {
                throw new ArgumentException("Invoice position prices and tax rate cannot be negative.", nameof(positions));
            }
        }
    }

    private AuditLog CreateAuditLog(string entityName, int entityId, string actionName, string oldValues, string newValues, string summary)
        => new(entityName, entityId, actionName, currentUserAccessor.UserId, oldValues, newValues, summary)
        {
            ChangedAtUtc = DateTime.UtcNow
        };

    private static PaymentDirection ParsePaymentDirection(string value)
    {
        if (!Enum.TryParse<PaymentDirection>(value, ignoreCase: true, out var parsed))
        {
            throw new ArgumentException($"Unknown payment direction '{value}'.", nameof(value));
        }

        return parsed;
    }

    private static IReadOnlyList<FinanceStatusOptionView> GetCostStatuses()
        => Enum.GetValues<CostDocumentStatus>()
            .Select(item => new FinanceStatusOptionView(item.ToString(), item switch
            {
                CostDocumentStatus.Draft => "Szkic",
                CostDocumentStatus.Approved => "Zatwierdzony",
                CostDocumentStatus.Cancelled => "Anulowany",
                _ => item.ToString()
            }))
            .ToArray();

    private static IReadOnlyList<FinanceStatusOptionView> GetInvoiceStatuses()
        => Enum.GetValues<InvoiceStatus>()
            .Select(item => new FinanceStatusOptionView(item.ToString(), item switch
            {
                InvoiceStatus.Issued => "Wystawiona",
                InvoiceStatus.PartiallyPaid => "Częściowo rozliczona",
                InvoiceStatus.Paid => "Rozliczona",
                InvoiceStatus.Cancelled => "Anulowana",
                _ => item.ToString()
            }))
            .ToArray();

    private static IReadOnlyList<FinancePaymentDirectionOptionView> GetPaymentDirections()
        => Enum.GetValues<PaymentDirection>()
            .Select(item => new FinancePaymentDirectionOptionView(item.ToString(), item switch
            {
                PaymentDirection.Incoming => "Wpływ",
                PaymentDirection.Outgoing => "Wypłata",
                _ => item.ToString()
            }))
            .ToArray();

    private static string NormalizeRequired(string? value, string fieldName)
    {
        if (value is null)
        {
            throw new ArgumentException($"{fieldName} is required.", fieldName);
        }

        var normalized = value.Trim();
        if (normalized.Length == 0)
        {
            throw new ArgumentException($"{fieldName} is required.", fieldName);
        }

        return normalized;
    }

    private static string? NormalizeOptional(string? value)
    {
        if (value is null)
        {
            return null;
        }

        var normalized = value.Trim();
        return normalized.Length == 0 ? null : normalized;
    }

    private static string Serialize<T>(T value) => JsonSerializer.Serialize(value, SerializerOptions);
}
