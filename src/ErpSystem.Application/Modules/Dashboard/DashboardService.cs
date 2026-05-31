namespace ErpSystem.Application.Modules.Dashboard;

public sealed class DashboardService : IDashboardService
{
    private readonly IDashboardRepository repository;

    public DashboardService(IDashboardRepository repository)
    {
        this.repository = repository;
    }

    public async Task<DashboardSummaryView> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var productCount = await repository.CountProductsAsync(cancellationToken);
        var draftWarehouseDocumentCount = await repository.CountDraftWarehouseDocumentsAsync(cancellationToken);
        var transportOrderCount = await repository.CountTransportOrdersAsync(cancellationToken);
        var activeEmployeeCount = await repository.CountActiveEmployeesAsync(cancellationToken);

        return new DashboardSummaryView(
            productCount,
            draftWarehouseDocumentCount,
            transportOrderCount,
            activeEmployeeCount);
    }
}
