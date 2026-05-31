namespace ErpSystem.Application.Modules.Dashboard;

public sealed record DashboardSummaryView(
    int ProductCount,
    int DraftWarehouseDocumentCount,
    int TransportOrderCount,
    int ActiveEmployeeCount);
