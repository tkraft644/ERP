using ErpSystem.Application.Common;
using ErpSystem.Application.Modules.Transport;

namespace ErpSystem.Api.Features.Transport;

public static class TransportFeatureEndpoints
{
    public static IEndpointRouteBuilder MapTransportFeatures(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/features/transport").RequireAuthorization();

        group.MapGet("/", () => Results.Ok(new
        {
            module = "Transport",
            sharedCore = true,
            orderTypes = new[]
            {
                "Domestic",
                "International"
            },
            folders = new[]
            {
                "Modules/Transport",
                "Views/Transport",
                "ViewModels/Transport",
                "Features/Transport"
            }
        }));

        group.MapGet("/reference-data", async (ITransportService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetReferenceDataAsync(cancellationToken)));

        group.MapGet("/orders", async (ITransportService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetOrdersAsync(cancellationToken)));

        group.MapGet("/orders/{orderId:int}", async (int orderId, ITransportService service, CancellationToken cancellationToken) =>
        {
            var order = await service.GetOrderAsync(orderId, cancellationToken);
            return order is null ? Results.NotFound() : Results.Ok(order);
        });

        group.MapPost("/orders", async (SaveTransportOrderRequest request, ITransportService service, CancellationToken cancellationToken) =>
        {
            try
            {
                var order = await service.CreateOrderAsync(request, cancellationToken);
                return Results.Created($"/features/transport/orders/{order.Id}", order);
            }
            catch (ArgumentException exception)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    [exception.ParamName ?? "request"] = [exception.Message]
                });
            }
            catch (InvalidOperationException exception)
            {
                return Results.BadRequest(new { message = exception.Message });
            }
        });

        group.MapPut("/orders/{orderId:int}", async (int orderId, SaveTransportOrderRequest request, ITransportService service, CancellationToken cancellationToken) =>
        {
            try
            {
                var order = await service.UpdateOrderAsync(orderId, request, cancellationToken);
                return order is null ? Results.NotFound() : Results.Ok(order);
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
                return Results.Conflict(new { message = exception.Message });
            }
            catch (InvalidOperationException exception)
            {
                return Results.BadRequest(new { message = exception.Message });
            }
        });

        group.MapPost("/orders/{orderId:int}/status", async (int orderId, ChangeTransportOrderStatusRequest request, ITransportService service, CancellationToken cancellationToken) =>
        {
            try
            {
                var order = await service.ChangeStatusAsync(orderId, request, cancellationToken);
                return order is null ? Results.NotFound() : Results.Ok(order);
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
                return Results.Conflict(new { message = exception.Message });
            }
            catch (InvalidOperationException exception)
            {
                return Results.BadRequest(new { message = exception.Message });
            }
        });

        return endpoints;
    }
}
