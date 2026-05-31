using ErpSystem.Application.Modules.Dashboard;

namespace ErpSystem.Api.Features.Dashboard;

public static class DashboardEndpoints
{
    public static IEndpointRouteBuilder MapDashboardFeatures(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/dashboard").RequireAuthorization();

        group.MapGet("/summary", async (IDashboardService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetSummaryAsync(cancellationToken)));

        return endpoints;
    }
}
