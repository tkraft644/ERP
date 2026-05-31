using ErpSystem.Application.Modules.System.Foundation;
using ErpSystem.Infrastructure.Persistence;

namespace ErpSystem.Api.Features.System;

public static class SystemFoundationEndpoints
{
    public static IEndpointRouteBuilder MapSystemFoundationFeatures(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/features/system/foundation").RequireAuthorization();

        group.MapGet("/users", async (ISystemFoundationService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetUsersAsync(cancellationToken)));

        group.MapGet("/users/{userId:int}/menu", async (int userId, ISystemFoundationService service, CancellationToken cancellationToken) =>
        {
            var user = await service.GetUserAsync(userId, cancellationToken);
            return user is null
                ? Results.NotFound()
                : Results.Ok(await service.GetMenuForUserAsync(userId, cancellationToken));
        });

        group.MapGet("/permissions", async (ISystemFoundationService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetPermissionsAsync(cancellationToken)));

        group.MapGet("/audit", async (ISystemFoundationService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetAuditLogsAsync(cancellationToken)));

        group.MapGet("/dictionaries", async (ISystemFoundationService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetDictionaryItemsAsync(cancellationToken)));

        group.MapGet("/attachments", async (ISystemFoundationService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetAttachmentsAsync(cancellationToken)));

        group.MapGet("/history", async (ISystemFoundationService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetDocumentHistoryAsync(cancellationToken)));

        group.MapGet("/statuses", async (ISystemFoundationService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetDocumentStatusesAsync(cancellationToken)));

        group.MapGet("/sequences", async (ISystemFoundationService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetDocumentNumberSequencesAsync(cancellationToken)));

        group.MapPost("/sequences/{key}/next", async (string key, ISystemFoundationService service, CancellationToken cancellationToken) =>
            Results.Ok(new { key, value = await service.GenerateDocumentNumberAsync(key, cancellationToken) }));

        return endpoints;
    }
}
