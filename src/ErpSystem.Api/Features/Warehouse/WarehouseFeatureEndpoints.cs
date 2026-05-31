using ErpSystem.Application.Common;
using ErpSystem.Application.Modules.Warehouse;

namespace ErpSystem.Api.Features.Warehouse;

public static class WarehouseFeatureEndpoints
{
    public static IEndpointRouteBuilder MapWarehouseFeatures(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/features/warehouse").RequireAuthorization();

        group.MapGet("/", () => Results.Ok(new
        {
            module = "Warehouse",
            folders = new[]
            {
                "Modules/Warehouse",
                "Views/Warehouse",
                "ViewModels/Warehouse",
                "Features/Warehouse"
            }
        }));

        group.MapGet("/reference-data", async (IWarehouseService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetReferenceDataAsync(cancellationToken)));

        group.MapGet("/document-types", (IWarehouseService service) =>
            Results.Ok(service.GetDocumentTypes()));

        group.MapGet("/stock", async (int? warehouseId, int? productId, IWarehouseService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetStockAsync(warehouseId, productId, cancellationToken)));

        group.MapGet("/products", async (IWarehouseService service, CancellationToken cancellationToken) =>
        {
            var referenceData = await service.GetReferenceDataAsync(cancellationToken);
            return Results.Ok(referenceData.Products);
        });

        group.MapPost("/products", async (SaveWarehouseProductRequest request, IWarehouseService service, CancellationToken cancellationToken) =>
        {
            try
            {
                var product = await service.CreateProductAsync(request, cancellationToken);
                return Results.Created($"/features/warehouse/products/{product.Id}", product);
            }
            catch (ArgumentException exception)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    [exception.ParamName ?? "request"] = [exception.Message]
                });
            }
        });

        group.MapGet("/documents", async (IWarehouseService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetDocumentsAsync(cancellationToken)));

        group.MapGet("/documents/{documentId:int}", async (int documentId, IWarehouseService service, CancellationToken cancellationToken) =>
        {
            var document = await service.GetDocumentAsync(documentId, cancellationToken);
            return document is null ? Results.NotFound() : Results.Ok(document);
        });

        group.MapPost("/documents", async (SaveWarehouseDocumentRequest request, IWarehouseService service, CancellationToken cancellationToken) =>
        {
            try
            {
                var document = await service.CreateDocumentAsync(request, cancellationToken);
                return Results.Created($"/features/warehouse/documents/{document.Id}", document);
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

        group.MapPut("/documents/{documentId:int}", async (int documentId, SaveWarehouseDocumentRequest request, IWarehouseService service, CancellationToken cancellationToken) =>
        {
            try
            {
                var document = await service.UpdateDocumentAsync(documentId, request, cancellationToken);
                return document is null ? Results.NotFound() : Results.Ok(document);
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

        group.MapPost("/documents/{documentId:int}/post", async (int documentId, PostWarehouseDocumentRequest request, IWarehouseService service, CancellationToken cancellationToken) =>
        {
            try
            {
                var document = await service.PostDocumentAsync(documentId, request, cancellationToken);
                return document is null ? Results.NotFound() : Results.Ok(document);
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

        group.MapPost("/documents/{documentId:int}/archive", async (int documentId, RowVersionRequest request, IWarehouseService service, CancellationToken cancellationToken) =>
        {
            try
            {
                var document = await service.ArchiveDocumentAsync(documentId, request, cancellationToken);
                return document is null ? Results.NotFound() : Results.Ok(document);
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

        group.MapPost("/documents/{documentId:int}/delete", async (int documentId, RowVersionRequest request, IWarehouseService service, CancellationToken cancellationToken) =>
        {
            try
            {
                await service.DeleteDocumentAsync(documentId, request, cancellationToken);
                return Results.NoContent();
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

        group.MapPut("/products/{productId:int}", async (int productId, SaveWarehouseProductRequest request, IWarehouseService service, CancellationToken cancellationToken) =>
        {
            try
            {
                var product = await service.UpdateProductAsync(productId, request, cancellationToken);
                return product is null ? Results.NotFound() : Results.Ok(product);
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

        group.MapPost("/products/{productId:int}/delete", async (int productId, RowVersionRequest request, IWarehouseService service, CancellationToken cancellationToken) =>
        {
            try
            {
                await service.DeleteProductAsync(productId, request, cancellationToken);
                return Results.NoContent();
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
