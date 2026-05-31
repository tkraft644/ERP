namespace ErpSystem.Domain.Modules.Transport;

public sealed class TransportOrderRoute : Common.AuditableEntity
{
    private TransportOrderRoute()
    {
    }

    public TransportOrderRoute(
        decimal? plannedDistanceKm,
        decimal? plannedRevenue,
        DateTime? plannedLoadingAtUtc,
        DateTime? plannedUnloadingAtUtc,
        string? routeSummary)
    {
        PlannedDistanceKm = plannedDistanceKm;
        PlannedRevenue = plannedRevenue;
        PlannedLoadingAtUtc = plannedLoadingAtUtc;
        PlannedUnloadingAtUtc = plannedUnloadingAtUtc;
        RouteSummary = routeSummary;
    }

    public int TransportOrderId { get; private set; }
    public TransportOrder TransportOrder { get; private set; } = null!;
    public decimal? PlannedDistanceKm { get; private set; }
    public decimal? PlannedRevenue { get; private set; }
    public DateTime? PlannedLoadingAtUtc { get; private set; }
    public DateTime? PlannedUnloadingAtUtc { get; private set; }
    public string? RouteSummary { get; private set; }

    internal void AssignTo(TransportOrder order)
    {
        TransportOrder = order;
        TransportOrderId = order.Id;
    }
}
