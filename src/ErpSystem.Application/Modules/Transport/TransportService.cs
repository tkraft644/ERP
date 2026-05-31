using System.Text.Json;
using ErpSystem.Application.Common;
using ErpSystem.Domain.Modules.HR;
using ErpSystem.Domain.Modules.System.Auditing;
using ErpSystem.Domain.Modules.Transport;

namespace ErpSystem.Application.Modules.Transport;

public sealed class TransportService : ITransportService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly ITransportRepository repository;
    private readonly ICurrentUserAccessor currentUserAccessor;

    public TransportService(ITransportRepository repository, ICurrentUserAccessor currentUserAccessor)
    {
        this.repository = repository;
        this.currentUserAccessor = currentUserAccessor;
    }

    public async Task<TransportReferenceDataView> GetReferenceDataAsync(CancellationToken cancellationToken = default)
    {
        var partners = await repository.GetPartnersAsync(cancellationToken);
        var carriers = await repository.GetCarriersAsync(cancellationToken);
        var vehicles = await repository.GetVehiclesAsync(cancellationToken);
        var trailers = await repository.GetTrailersAsync(cancellationToken);
        var drivers = await repository.GetDriversAsync(cancellationToken);

        return new TransportReferenceDataView(
            partners.OrderBy(item => item.Name)
                .Select(item => new TransportPartnerOptionView(item.Id, item.Code, item.Name))
                .ToArray(),
            carriers.OrderBy(item => item.Contractor.Name)
                .Select(item => new CarrierOptionView(item.Id, item.Code, item.ContractorId, item.Contractor.Name, item.IsPreferred))
                .ToArray(),
            vehicles.OrderBy(item => item.RegistrationNumber)
                .Select(item => new VehicleOptionView(item.Id, item.RegistrationNumber, $"{item.RegistrationNumber} • {item.Brand} {item.Model}", item.CarrierId))
                .ToArray(),
            trailers.OrderBy(item => item.RegistrationNumber)
                .Select(item => new TrailerOptionView(item.Id, item.RegistrationNumber, item.TrailerType, item.CarrierId))
                .ToArray(),
            drivers.OrderBy(item => item.Employee.LastName).ThenBy(item => item.Employee.FirstName)
                .Select(item => new DriverOptionView(item.Id, item.Employee.FullName, item.Employee.PhoneNumber, null))
                .ToArray(),
            GetOrderTypeOptions(),
            GetStatusOptions(),
            GetStopTypeOptions(),
            GetDocumentTypeOptions());
    }

    public async Task<IReadOnlyList<TransportOrderListItemView>> GetOrdersAsync(CancellationToken cancellationToken = default)
    {
        var orders = await repository.GetOrdersAsync(cancellationToken);
        return orders
            .OrderByDescending(item => item.OrderDate)
            .ThenByDescending(item => item.Id)
            .Select(MapListItem)
            .ToArray();
    }

    public async Task<TransportOrderDetailsView?> GetOrderAsync(int orderId, CancellationToken cancellationToken = default)
    {
        var order = await repository.GetOrderAsync(orderId, cancellationToken);
        return order is null ? null : MapDetails(order);
    }

    public async Task<TransportOrderDetailsView> CreateOrderAsync(SaveTransportOrderRequest request, CancellationToken cancellationToken = default)
    {
        var type = ParseOrderType(request.OrderType);
        ValidateRequest(type, request);

        var number = await repository.GenerateDocumentNumberAsync("TR_ORD", DateTime.UtcNow, cancellationToken);
        var order = BuildOrder(number, type, request);

        await repository.AddOrderAsync(order, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        var createdOrder = await repository.GetOrderAsync(order.Id, cancellationToken)
            ?? throw new InvalidOperationException("Created transport order could not be reloaded.");
        await AddStatusHistoryAsync(createdOrder, TransportOrderStatus.New, "Order created.", cancellationToken);
        await repository.AddAuditLogAsync(CreateAuditLog(
            createdOrder.Id,
            "Created",
            "{}",
            Serialize(MapDetails(createdOrder)),
            $"Created transport order '{createdOrder.Number}'."), cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        var createdOrderWithHistory = await repository.GetOrderAsync(createdOrder.Id, cancellationToken)
            ?? throw new InvalidOperationException("Created transport order could not be reloaded after history update.");

        return MapDetails(createdOrderWithHistory);
    }

    public async Task<TransportOrderDetailsView?> UpdateOrderAsync(int orderId, SaveTransportOrderRequest request, CancellationToken cancellationToken = default)
    {
        if (request.RowVersion is null || request.RowVersion.Length == 0)
        {
            throw new ArgumentException("RowVersion is required when updating a transport order.", nameof(request));
        }

        var order = await repository.GetOrderForUpdateAsync(orderId, cancellationToken);
        if (order is null)
        {
            return null;
        }

        if (order.Status is TransportOrderStatus.Closed or TransportOrderStatus.Cancelled)
        {
            throw new InvalidOperationException("Closed or cancelled transport orders cannot be edited.");
        }

        var type = ParseOrderType(request.OrderType);
        ValidateRequest(type, request);

        repository.SetOriginalRowVersion(order, request.RowVersion);
        var previousView = MapDetails(order);

        order.UpdateCore(
            type,
            request.OrderDate,
            request.CarrierId,
            request.VehicleId,
            request.TrailerId,
            request.DriverId,
            NormalizeOptional(request.LoadingCountryCode)?.ToUpperInvariant(),
            NormalizeOptional(request.UnloadingCountryCode)?.ToUpperInvariant(),
            NormalizeOptional(request.Incoterms),
            NormalizeOptional(request.CurrencyCode)?.ToUpperInvariant(),
            request.ExchangeRate,
            request.RequiresCustomsClearance,
            request.RequiresCmrDocuments,
            NormalizeOptional(request.DomesticRegion),
            NormalizeOptional(request.DomesticTransportKind),
            NormalizeOptional(request.ExternalReference),
            NormalizeOptional(request.Notes));
        order.ReplaceRoute(BuildRoute(request.Route));
        order.ReplaceStops(BuildStops(request.Stops));
        order.ReplaceDocuments(BuildDocuments(request.Documents));
        order.ReplaceCosts(BuildCosts(request.Costs));

        await repository.SaveChangesAsync(cancellationToken);
        var updatedOrder = await repository.GetOrderAsync(order.Id, cancellationToken)
            ?? throw new InvalidOperationException("Updated transport order could not be reloaded.");
        await repository.AddAuditLogAsync(CreateAuditLog(
            updatedOrder.Id,
            "Updated",
            Serialize(previousView),
            Serialize(MapDetails(updatedOrder)),
            $"Updated transport order '{updatedOrder.Number}'."), cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return MapDetails(updatedOrder);
    }

    public async Task<TransportOrderDetailsView?> ChangeStatusAsync(int orderId, ChangeTransportOrderStatusRequest request, CancellationToken cancellationToken = default)
    {
        if (request.RowVersion.Length == 0)
        {
            throw new ArgumentException("RowVersion is required when changing transport order status.", nameof(request));
        }

        var order = await repository.GetOrderForUpdateAsync(orderId, cancellationToken);
        if (order is null)
        {
            return null;
        }

        repository.SetOriginalRowVersion(order, request.RowVersion);

        var targetStatus = ParseStatus(request.Status);
        EnsureStatusTransition(order.Status, targetStatus);
        var previousStatus = order.Status;
        order.ChangeStatus(targetStatus);

        await repository.SaveChangesAsync(cancellationToken);
        var updatedOrder = await repository.GetOrderAsync(order.Id, cancellationToken)
            ?? throw new InvalidOperationException("Updated transport order could not be reloaded.");
        await AddStatusHistoryAsync(updatedOrder, targetStatus, NormalizeOptional(request.Note), cancellationToken);
        await repository.AddAuditLogAsync(CreateAuditLog(
            updatedOrder.Id,
            "StatusChanged",
            $"{{\"previousStatus\":\"{previousStatus}\"}}",
            $"{{\"status\":\"{targetStatus}\"}}",
            $"Changed transport order '{updatedOrder.Number}' status to '{targetStatus}'."), cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        var updatedOrderWithHistory = await repository.GetOrderAsync(updatedOrder.Id, cancellationToken)
            ?? throw new InvalidOperationException("Updated transport order could not be reloaded after status change.");

        return MapDetails(updatedOrderWithHistory);
    }

    private async Task AddStatusHistoryAsync(
        TransportOrder order,
        TransportOrderStatus status,
        string? note,
        CancellationToken cancellationToken)
    {
        var entry = new TransportStatusHistory(status, currentUserAccessor.UserId, DateTime.UtcNow, note);
        entry.AssignTo(order);
        await repository.AddStatusHistoryAsync(entry, cancellationToken);
    }

    private static TransportOrder BuildOrder(string number, TransportOrderType type, SaveTransportOrderRequest request)
    {
        var order = new TransportOrder(
            number,
            type,
            request.OrderDate,
            request.CarrierId,
            request.VehicleId,
            request.TrailerId,
            request.DriverId,
            NormalizeOptional(request.LoadingCountryCode)?.ToUpperInvariant(),
            NormalizeOptional(request.UnloadingCountryCode)?.ToUpperInvariant(),
            NormalizeOptional(request.Incoterms),
            NormalizeOptional(request.CurrencyCode)?.ToUpperInvariant(),
            request.ExchangeRate,
            request.RequiresCustomsClearance,
            request.RequiresCmrDocuments,
            NormalizeOptional(request.DomesticRegion),
            NormalizeOptional(request.DomesticTransportKind),
            NormalizeOptional(request.ExternalReference),
            NormalizeOptional(request.Notes));
        order.ReplaceRoute(BuildRoute(request.Route));
        order.ReplaceStops(BuildStops(request.Stops));
        order.ReplaceDocuments(BuildDocuments(request.Documents));
        order.ReplaceCosts(BuildCosts(request.Costs));
        return order;
    }

    private static TransportOrderRoute BuildRoute(SaveTransportRouteRequest request)
        => new(request.PlannedDistanceKm, request.PlannedRevenue, request.PlannedLoadingAtUtc, request.PlannedUnloadingAtUtc, NormalizeOptional(request.RouteSummary));

    private static IReadOnlyList<TransportStop> BuildStops(IReadOnlyList<SaveTransportStopRequest> requests)
        => requests
            .OrderBy(item => item.Sequence)
            .Select(item => new TransportStop(
                item.Sequence,
                ParseStopType(item.StopType),
                item.ContractorId,
                NormalizeRequired(item.Name, nameof(item.Name)),
                NormalizeRequired(item.CountryCode, nameof(item.CountryCode)).ToUpperInvariant(),
                NormalizeRequired(item.City, nameof(item.City)),
                NormalizeRequired(item.AddressLine, nameof(item.AddressLine)),
                item.PlannedAtUtc,
                item.ActualAtUtc,
                NormalizeOptional(item.Notes)))
            .ToArray();

    private static IReadOnlyList<TransportDocument> BuildDocuments(IReadOnlyList<SaveTransportDocumentRequest> requests)
        => requests
            .Select(item => new TransportDocument(
                ParseDocumentType(item.DocumentType),
                NormalizeOptional(item.DocumentNumber),
                NormalizeOptional(item.FileName),
                item.IssuedAtUtc,
                item.ReceivedAtUtc,
                item.IsRequired))
            .ToArray();

    private static IReadOnlyList<TransportCost> BuildCosts(IReadOnlyList<SaveTransportCostRequest> requests)
        => requests
            .Select(item => new TransportCost(
                NormalizeRequired(item.CostType, nameof(item.CostType)),
                NormalizeRequired(item.Description, nameof(item.Description)),
                NormalizeRequired(item.CurrencyCode, nameof(item.CurrencyCode)).ToUpperInvariant(),
                item.Amount,
                item.ExchangeRate,
                item.IsLocalCost))
            .ToArray();

    private AuditLog CreateAuditLog(int orderId, string actionName, string oldValues, string newValues, string summary)
        => new("TransportOrder", orderId, actionName, currentUserAccessor.UserId, oldValues, newValues, summary)
        {
            ChangedAtUtc = DateTime.UtcNow
        };

    private static TransportOrderListItemView MapListItem(TransportOrder order)
        => new(
            order.Id,
            order.Number,
            order.OrderType.ToString(),
            order.Status.ToString(),
            order.OrderDate,
            order.Carrier?.Contractor.Name,
            order.Driver?.Employee.FullName,
            order.Vehicle?.RegistrationNumber,
            order.LoadingCountryCode,
            order.UnloadingCountryCode,
            order.DomesticRegion,
            order.CurrencyCode,
            order.Costs.Sum(item => item.Amount),
            order.RowVersion);

    private static TransportOrderDetailsView MapDetails(TransportOrder order)
        => new(
            order.Id,
            order.Number,
            order.OrderType.ToString(),
            order.Status.ToString(),
            order.OrderDate,
            order.CarrierId,
            order.Carrier?.Contractor.Name,
            order.VehicleId,
            order.Vehicle?.RegistrationNumber,
            order.TrailerId,
            order.Trailer?.RegistrationNumber,
            order.DriverId,
            order.Driver?.Employee.FullName,
            order.LoadingCountryCode,
            order.UnloadingCountryCode,
            order.Incoterms,
            order.CurrencyCode,
            order.ExchangeRate,
            order.RequiresCustomsClearance,
            order.RequiresCmrDocuments,
            order.DomesticRegion,
            order.DomesticTransportKind,
            order.ExternalReference,
            order.Notes,
            order.Route is null
                ? null
                : new TransportRouteView(
                    order.Route.Id,
                    order.Route.PlannedDistanceKm,
                    order.Route.PlannedRevenue,
                    order.Route.PlannedLoadingAtUtc,
                    order.Route.PlannedUnloadingAtUtc,
                    order.Route.RouteSummary),
            order.Stops
                .OrderBy(item => item.Sequence)
                .Select(item => new TransportStopView(
                    item.Id,
                    item.Sequence,
                    item.StopType.ToString(),
                    item.ContractorId,
                    item.Contractor?.Name,
                    item.Name,
                    item.CountryCode,
                    item.City,
                    item.AddressLine,
                    item.PlannedAtUtc,
                    item.ActualAtUtc,
                    item.Notes))
                .ToArray(),
            order.Documents
                .OrderBy(item => item.DocumentType)
                .ThenBy(item => item.Id)
                .Select(item => new TransportDocumentView(
                    item.Id,
                    item.DocumentType.ToString(),
                    item.DocumentNumber,
                    item.FileName,
                    item.IssuedAtUtc,
                    item.ReceivedAtUtc,
                    item.IsRequired))
                .ToArray(),
            order.Costs
                .OrderBy(item => item.CostType)
                .ThenBy(item => item.Description)
                .Select(item => new TransportCostView(
                    item.Id,
                    item.CostType,
                    item.Description,
                    item.CurrencyCode,
                    item.Amount,
                    item.ExchangeRate,
                    item.IsLocalCost))
                .ToArray(),
            order.StatusHistory
                .OrderByDescending(item => item.ChangedAtUtc)
                .Select(item => new TransportStatusHistoryView(
                    item.Id,
                    item.Status.ToString(),
                    item.ChangedByUserId,
                    item.ChangedAtUtc,
                    item.Note))
                .ToArray(),
            order.RowVersion);

    private static IReadOnlyList<TransportOrderTypeOptionView> GetOrderTypeOptions()
        => Enum.GetValues<TransportOrderType>()
            .Select(item => new TransportOrderTypeOptionView(item.ToString(), item switch
            {
                TransportOrderType.Domestic => "Krajowe",
                TransportOrderType.International => "Zagraniczne",
                _ => item.ToString()
            }))
            .ToArray();

    private static IReadOnlyList<TransportStatusOptionView> GetStatusOptions()
        => Enum.GetValues<TransportOrderStatus>()
            .Select(item => new TransportStatusOptionView(item.ToString(), item switch
            {
                TransportOrderStatus.New => "Nowe",
                TransportOrderStatus.Accepted => "Przyjęte",
                TransportOrderStatus.Planned => "Zaplanowane",
                TransportOrderStatus.InProgress => "W realizacji",
                TransportOrderStatus.Loaded => "Załadowane",
                TransportOrderStatus.Delivered => "Dostarczone",
                TransportOrderStatus.Closed => "Zamknięte",
                TransportOrderStatus.Cancelled => "Anulowane",
                _ => item.ToString()
            }))
            .ToArray();

    private static IReadOnlyList<TransportStopTypeOptionView> GetStopTypeOptions()
        => Enum.GetValues<TransportStopType>()
            .Select(item => new TransportStopTypeOptionView(item.ToString(), item switch
            {
                TransportStopType.Loading => "Załadunek",
                TransportStopType.Unloading => "Rozładunek",
                TransportStopType.Customs => "Odprawa celna",
                TransportStopType.Other => "Inny punkt",
                _ => item.ToString()
            }))
            .ToArray();

    private static IReadOnlyList<TransportDocumentTypeOptionView> GetDocumentTypeOptions()
        => Enum.GetValues<TransportDocumentType>()
            .Select(item => new TransportDocumentTypeOptionView(item.ToString(), item switch
            {
                TransportDocumentType.Cmr => "CMR",
                TransportDocumentType.Customs => "Dokument celny",
                TransportDocumentType.Invoice => "Faktura",
                TransportDocumentType.Other => "Inny dokument",
                _ => item.ToString()
            }))
            .ToArray();

    private static void EnsureStatusTransition(TransportOrderStatus current, TransportOrderStatus target)
    {
        if (current == target)
        {
            throw new InvalidOperationException("Transport order already has the requested status.");
        }

        var allowed = current switch
        {
            TransportOrderStatus.New => target is TransportOrderStatus.Accepted or TransportOrderStatus.Cancelled,
            TransportOrderStatus.Accepted => target is TransportOrderStatus.Planned or TransportOrderStatus.Cancelled,
            TransportOrderStatus.Planned => target is TransportOrderStatus.InProgress or TransportOrderStatus.Cancelled,
            TransportOrderStatus.InProgress => target is TransportOrderStatus.Loaded or TransportOrderStatus.Cancelled,
            TransportOrderStatus.Loaded => target is TransportOrderStatus.Delivered or TransportOrderStatus.Cancelled,
            TransportOrderStatus.Delivered => target == TransportOrderStatus.Closed,
            _ => false
        };

        if (!allowed)
        {
            throw new InvalidOperationException($"Transition from '{current}' to '{target}' is not allowed.");
        }
    }

    private static void ValidateRequest(TransportOrderType type, SaveTransportOrderRequest request)
    {
        if (request.Stops.Count < 2)
        {
            throw new ArgumentException("Transport order should contain at least two stops.", nameof(request.Stops));
        }

        if (request.Stops.Select(item => item.Sequence).Distinct().Count() != request.Stops.Count)
        {
            throw new ArgumentException("Transport stop sequence values must be unique.", nameof(request.Stops));
        }

        if (type == TransportOrderType.International)
        {
            _ = NormalizeRequired(request.LoadingCountryCode, nameof(request.LoadingCountryCode));
            _ = NormalizeRequired(request.UnloadingCountryCode, nameof(request.UnloadingCountryCode));
            _ = NormalizeRequired(request.Incoterms, nameof(request.Incoterms));
            _ = NormalizeRequired(request.CurrencyCode, nameof(request.CurrencyCode));
            if (request.ExchangeRate is null or <= 0m)
            {
                throw new ArgumentException("ExchangeRate is required for international transport orders.", nameof(request.ExchangeRate));
            }
        }
        else
        {
            _ = NormalizeRequired(request.DomesticRegion, nameof(request.DomesticRegion));
            _ = NormalizeRequired(request.DomesticTransportKind, nameof(request.DomesticTransportKind));
        }
    }

    private static TransportOrderType ParseOrderType(string value)
    {
        if (!Enum.TryParse<TransportOrderType>(value, ignoreCase: true, out var parsed))
        {
            throw new ArgumentException($"Unknown transport order type '{value}'.", nameof(value));
        }

        return parsed;
    }

    private static TransportOrderStatus ParseStatus(string value)
    {
        if (!Enum.TryParse<TransportOrderStatus>(value, ignoreCase: true, out var parsed))
        {
            throw new ArgumentException($"Unknown transport order status '{value}'.", nameof(value));
        }

        return parsed;
    }

    private static TransportStopType ParseStopType(string value)
    {
        if (!Enum.TryParse<TransportStopType>(value, ignoreCase: true, out var parsed))
        {
            throw new ArgumentException($"Unknown transport stop type '{value}'.", nameof(value));
        }

        return parsed;
    }

    private static TransportDocumentType ParseDocumentType(string value)
    {
        if (!Enum.TryParse<TransportDocumentType>(value, ignoreCase: true, out var parsed))
        {
            throw new ArgumentException($"Unknown transport document type '{value}'.", nameof(value));
        }

        return parsed;
    }

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
