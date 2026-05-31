using ErpSystem.Application.Common;
using ErpSystem.Application.Modules.Finance;

namespace ErpSystem.Api.Features.Finance;

public static class FinanceFeatureEndpoints
{
    public static IEndpointRouteBuilder MapFinanceFeatures(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/features/finance").RequireAuthorization();

        group.MapGet("/", () => Results.Ok(new
        {
            module = "Finance / Costs",
            folders = new[]
            {
                "Modules/Finance",
                "Views/Finance",
                "ViewModels/Finance",
                "Features/Finance"
            }
        }));

        group.MapGet("/reference-data", async (IFinanceService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetReferenceDataAsync(cancellationToken)));

        group.MapGet("/cost-documents", async (IFinanceService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetCostDocumentsAsync(cancellationToken)));

        group.MapGet("/cost-documents/{costDocumentId:int}", async (int costDocumentId, IFinanceService service, CancellationToken cancellationToken) =>
        {
            var document = await service.GetCostDocumentAsync(costDocumentId, cancellationToken);
            return document is null ? Results.NotFound() : Results.Ok(document);
        });

        group.MapPost("/cost-documents", async (SaveCostDocumentRequest request, IFinanceService service, CancellationToken cancellationToken) =>
        {
            try
            {
                var document = await service.CreateCostDocumentAsync(request, cancellationToken);
                return Results.Created($"/features/finance/cost-documents/{document.Id}", document);
            }
            catch (ArgumentException exception)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    [exception.ParamName ?? "request"] = [exception.Message]
                });
            }
        });

        group.MapPut("/cost-documents/{costDocumentId:int}", async (int costDocumentId, SaveCostDocumentRequest request, IFinanceService service, CancellationToken cancellationToken) =>
        {
            try
            {
                var document = await service.UpdateCostDocumentAsync(costDocumentId, request, cancellationToken);
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
        });

        group.MapGet("/invoices", async (IFinanceService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetInvoicesAsync(cancellationToken)));

        group.MapGet("/invoices/{invoiceId:int}", async (int invoiceId, IFinanceService service, CancellationToken cancellationToken) =>
        {
            var invoice = await service.GetInvoiceAsync(invoiceId, cancellationToken);
            return invoice is null ? Results.NotFound() : Results.Ok(invoice);
        });

        group.MapPost("/invoices", async (SaveInvoiceRequest request, IFinanceService service, CancellationToken cancellationToken) =>
        {
            try
            {
                var invoice = await service.CreateInvoiceAsync(request, cancellationToken);
                return Results.Created($"/features/finance/invoices/{invoice.Id}", invoice);
            }
            catch (ArgumentException exception)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    [exception.ParamName ?? "request"] = [exception.Message]
                });
            }
        });

        group.MapPut("/invoices/{invoiceId:int}", async (int invoiceId, SaveInvoiceRequest request, IFinanceService service, CancellationToken cancellationToken) =>
        {
            try
            {
                var invoice = await service.UpdateInvoiceAsync(invoiceId, request, cancellationToken);
                return invoice is null ? Results.NotFound() : Results.Ok(invoice);
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
        });

        group.MapGet("/payments", async (IFinanceService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetPaymentsAsync(cancellationToken)));

        group.MapPost("/payments", async (SavePaymentRequest request, IFinanceService service, CancellationToken cancellationToken) =>
        {
            try
            {
                var payment = await service.CreatePaymentAsync(request, cancellationToken);
                return Results.Created($"/features/finance/payments/{payment.Id}", payment);
            }
            catch (ArgumentException exception)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    [exception.ParamName ?? "request"] = [exception.Message]
                });
            }
        });

        group.MapGet("/currency-rates", async (IFinanceService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetCurrencyRatesAsync(cancellationToken)));

        group.MapPost("/currency-rates", async (SaveCurrencyRateRequest request, IFinanceService service, CancellationToken cancellationToken) =>
        {
            try
            {
                var rate = await service.CreateCurrencyRateAsync(request, cancellationToken);
                return Results.Created($"/features/finance/currency-rates/{rate.Id}", rate);
            }
            catch (ArgumentException exception)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    [exception.ParamName ?? "request"] = [exception.Message]
                });
            }
        });

        group.MapPut("/currency-rates/{currencyRateId:int}", async (int currencyRateId, SaveCurrencyRateRequest request, IFinanceService service, CancellationToken cancellationToken) =>
        {
            try
            {
                var rate = await service.UpdateCurrencyRateAsync(currencyRateId, request, cancellationToken);
                return rate is null ? Results.NotFound() : Results.Ok(rate);
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
        });

        group.MapGet("/settlements", async (IFinanceService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetSettlementsAsync(cancellationToken)));

        group.MapPost("/settlements", async (SaveSettlementRequest request, IFinanceService service, CancellationToken cancellationToken) =>
        {
            try
            {
                var settlement = await service.CreateSettlementAsync(request, cancellationToken);
                return Results.Created($"/features/finance/settlements/{settlement.Id}", settlement);
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

        return endpoints;
    }
}
