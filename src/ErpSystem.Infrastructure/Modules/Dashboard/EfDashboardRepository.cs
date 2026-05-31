using ErpSystem.Application.Modules.Dashboard;
using ErpSystem.Domain.Modules.Warehouse;
using ErpSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ErpSystem.Infrastructure.Modules.Dashboard;

public sealed class EfDashboardRepository : IDashboardRepository
{
    private readonly ErpSystemDbContext dbContext;

    public EfDashboardRepository(ErpSystemDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Task<int> CountProductsAsync(CancellationToken cancellationToken = default)
        => dbContext.Products.AsNoTracking().CountAsync(cancellationToken);

    public Task<int> CountDraftWarehouseDocumentsAsync(CancellationToken cancellationToken = default)
        => dbContext.WarehouseDocuments.AsNoTracking().CountAsync(item => item.Status == WarehouseDocumentStatus.Draft, cancellationToken);

    public Task<int> CountTransportOrdersAsync(CancellationToken cancellationToken = default)
        => dbContext.TransportOrders.AsNoTracking().CountAsync(cancellationToken);

    public Task<int> CountActiveEmployeesAsync(CancellationToken cancellationToken = default)
        => dbContext.Employees.AsNoTracking().CountAsync(item => item.IsActive, cancellationToken);
}
