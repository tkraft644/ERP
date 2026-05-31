namespace ErpSystem.Application.Modules.Dashboard;

public interface IDashboardService
{
    Task<DashboardSummaryView> GetSummaryAsync(CancellationToken cancellationToken = default);
}
