using ErpSystem.Application.Common;
using ErpSystem.Application.Modules.Contractors;

namespace ErpSystem.Api.Features.Contractors;

public static class ContractorsFeatureEndpoints
{
    public static IEndpointRouteBuilder MapContractorsFeatures(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/features/contractors").RequireAuthorization();

        group.MapGet("/", () => Results.Ok(new
        {
            module = "Contractors",
            folders = new[]
            {
                "Modules/Contractors",
                "Views/Contractors",
                "ViewModels/Contractors",
                "Features/Contractors"
            }
        }));

        group.MapGet("/types", (IContractorService service) =>
            Results.Ok(service.GetAvailableContractorTypes()));

        group.MapGet("/address-kinds", (IContractorService service) =>
            Results.Ok(service.GetAvailableAddressKinds()));

        group.MapGet("/list", async (bool? includeInactive, IContractorService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetContractorsAsync(includeInactive ?? false, cancellationToken)));

        group.MapGet("/{contractorId:int}", async (int contractorId, IContractorService service, CancellationToken cancellationToken) =>
        {
            var contractor = await service.GetContractorAsync(contractorId, cancellationToken);
            return contractor is null ? Results.NotFound() : Results.Ok(contractor);
        });

        group.MapGet("/{contractorId:int}/history", async (int contractorId, IContractorService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetHistoryAsync(contractorId, cancellationToken)));

        group.MapPost("/", async (SaveContractorRequest request, IContractorService service, CancellationToken cancellationToken) =>
        {
            try
            {
                var contractor = await service.CreateContractorAsync(request, cancellationToken);
                return Results.Created($"/features/contractors/{contractor.Id}", contractor);
            }
            catch (ArgumentException exception)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    [exception.ParamName ?? "request"] = [exception.Message]
                });
            }
        });

        group.MapPut("/{contractorId:int}", async (int contractorId, SaveContractorRequest request, IContractorService service, CancellationToken cancellationToken) =>
        {
            try
            {
                var contractor = await service.UpdateContractorAsync(contractorId, request, cancellationToken);
                return contractor is null ? Results.NotFound() : Results.Ok(contractor);
            }
            catch (ArgumentException exception)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    [exception.ParamName ?? "request"] = [exception.Message]
                });
            }
            catch (ConcurrencyConflictException exception)
            {
                return Results.Conflict(new
                {
                    message = exception.Message
                });
            }
        });

        return endpoints;
    }
}
