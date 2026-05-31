using ErpSystem.Domain.Modules.HR;

namespace ErpSystem.Domain.Modules.Transport;

public sealed class TransportOrder : Common.AuditableEntity
{
    private TransportOrder()
    {
    }

    public TransportOrder(
        string number,
        TransportOrderType orderType,
        DateTime orderDate,
        int? carrierId,
        int? vehicleId,
        int? trailerId,
        int? driverId,
        string? loadingCountryCode,
        string? unloadingCountryCode,
        string? incoterms,
        string? currencyCode,
        decimal? exchangeRate,
        bool requiresCustomsClearance,
        bool requiresCmrDocuments,
        string? domesticRegion,
        string? domesticTransportKind,
        string? externalReference,
        string? notes)
    {
        Number = number;
        OrderType = orderType;
        OrderDate = orderDate;
        CarrierId = carrierId;
        VehicleId = vehicleId;
        TrailerId = trailerId;
        DriverId = driverId;
        LoadingCountryCode = loadingCountryCode;
        UnloadingCountryCode = unloadingCountryCode;
        Incoterms = incoterms;
        CurrencyCode = currencyCode;
        ExchangeRate = exchangeRate;
        RequiresCustomsClearance = requiresCustomsClearance;
        RequiresCmrDocuments = requiresCmrDocuments;
        DomesticRegion = domesticRegion;
        DomesticTransportKind = domesticTransportKind;
        ExternalReference = externalReference;
        Notes = notes;
        Status = TransportOrderStatus.New;
    }

    public string Number { get; private set; } = string.Empty;
    public TransportOrderType OrderType { get; private set; }
    public TransportOrderStatus Status { get; private set; }
    public DateTime OrderDate { get; private set; }
    public int? CarrierId { get; private set; }
    public Carrier? Carrier { get; private set; }
    public int? VehicleId { get; private set; }
    public Vehicle? Vehicle { get; private set; }
    public int? TrailerId { get; private set; }
    public Trailer? Trailer { get; private set; }
    public int? DriverId { get; private set; }
    public DriverProfile? Driver { get; private set; }
    public string? LoadingCountryCode { get; private set; }
    public string? UnloadingCountryCode { get; private set; }
    public string? Incoterms { get; private set; }
    public string? CurrencyCode { get; private set; }
    public decimal? ExchangeRate { get; private set; }
    public bool RequiresCustomsClearance { get; private set; }
    public bool RequiresCmrDocuments { get; private set; }
    public string? DomesticRegion { get; private set; }
    public string? DomesticTransportKind { get; private set; }
    public string? ExternalReference { get; private set; }
    public string? Notes { get; private set; }

    public TransportOrderRoute? Route { get; private set; }
    public List<TransportStop> Stops { get; private set; } = [];
    public List<TransportDocument> Documents { get; private set; } = [];
    public List<TransportCost> Costs { get; private set; } = [];
    public List<TransportStatusHistory> StatusHistory { get; private set; } = [];

    public void UpdateCore(
        TransportOrderType orderType,
        DateTime orderDate,
        int? carrierId,
        int? vehicleId,
        int? trailerId,
        int? driverId,
        string? loadingCountryCode,
        string? unloadingCountryCode,
        string? incoterms,
        string? currencyCode,
        decimal? exchangeRate,
        bool requiresCustomsClearance,
        bool requiresCmrDocuments,
        string? domesticRegion,
        string? domesticTransportKind,
        string? externalReference,
        string? notes)
    {
        OrderType = orderType;
        OrderDate = orderDate;
        CarrierId = carrierId;
        VehicleId = vehicleId;
        TrailerId = trailerId;
        DriverId = driverId;
        LoadingCountryCode = loadingCountryCode;
        UnloadingCountryCode = unloadingCountryCode;
        Incoterms = incoterms;
        CurrencyCode = currencyCode;
        ExchangeRate = exchangeRate;
        RequiresCustomsClearance = requiresCustomsClearance;
        RequiresCmrDocuments = requiresCmrDocuments;
        DomesticRegion = domesticRegion;
        DomesticTransportKind = domesticTransportKind;
        ExternalReference = externalReference;
        Notes = notes;
    }

    public void ReplaceRoute(TransportOrderRoute route)
    {
        route.AssignTo(this);
        Route = route;
    }

    public void ReplaceStops(IEnumerable<TransportStop> items)
    {
        Stops.Clear();
        foreach (var item in items)
        {
            item.AssignTo(this);
            Stops.Add(item);
        }
    }

    public void ReplaceDocuments(IEnumerable<TransportDocument> items)
    {
        Documents.Clear();
        foreach (var item in items)
        {
            item.AssignTo(this);
            Documents.Add(item);
        }
    }

    public void ReplaceCosts(IEnumerable<TransportCost> items)
    {
        Costs.Clear();
        foreach (var item in items)
        {
            item.AssignTo(this);
            Costs.Add(item);
        }
    }

    public void ChangeStatus(TransportOrderStatus status)
    {
        Status = status;
    }
}
