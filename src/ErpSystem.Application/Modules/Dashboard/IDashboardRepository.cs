namespace ErpSystem.Application.Modules.Dashboard;

public interface IDashboardRepository
{
    Task<int> CountProductsAsync(CancellationToken cancellationToken = default);
    Task<int> CountDraftWarehouseDocumentsAsync(CancellationToken cancellationToken = default);
    Task<int> CountTransportOrdersAsync(CancellationToken cancellationToken = default);
    Task<int> CountActiveEmployeesAsync(CancellationToken cancellationToken = default);
}
