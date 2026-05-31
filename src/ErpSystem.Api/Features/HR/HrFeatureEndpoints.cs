using ErpSystem.Application.Common;
using ErpSystem.Application.Modules.HR;

namespace ErpSystem.Api.Features.HR;

public static class HrFeatureEndpoints
{
    public static IEndpointRouteBuilder MapHrFeatures(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/features/hr").RequireAuthorization();

        group.MapGet("/", () => Results.Ok(new
        {
            module = "HR",
            folders = new[]
            {
                "Modules/HR",
                "Views/HR",
                "ViewModels/HR",
                "Features/HR"
            }
        }));

        group.MapGet("/reference-data", async (IHrService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetReferenceDataAsync(cancellationToken)));

        group.MapGet("/employees", async (bool? includeInactive, IHrService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetEmployeesAsync(includeInactive ?? false, cancellationToken)));

        group.MapGet("/employees/{employeeId:int}", async (int employeeId, IHrService service, CancellationToken cancellationToken) =>
        {
            var employee = await service.GetEmployeeAsync(employeeId, cancellationToken);
            return employee is null ? Results.NotFound() : Results.Ok(employee);
        });

        group.MapPost("/employees", async (SaveEmployeeRequest request, IHrService service, CancellationToken cancellationToken) =>
        {
            try
            {
                var employee = await service.CreateEmployeeAsync(request, cancellationToken);
                return Results.Created($"/features/hr/employees/{employee.Id}", employee);
            }
            catch (ArgumentException exception)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    [exception.ParamName ?? "request"] = [exception.Message]
                });
            }
        });

        group.MapPut("/employees/{employeeId:int}", async (int employeeId, SaveEmployeeRequest request, IHrService service, CancellationToken cancellationToken) =>
        {
            try
            {
                var employee = await service.UpdateEmployeeAsync(employeeId, request, cancellationToken);
                return employee is null ? Results.NotFound() : Results.Ok(employee);
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

        group.MapGet("/leave-requests", async (int? employeeId, string? status, IHrService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetLeaveRequestsAsync(employeeId, status, cancellationToken)));

        group.MapPost("/leave-requests", async (SaveLeaveRequestRequest request, IHrService service, CancellationToken cancellationToken) =>
        {
            try
            {
                var leaveRequest = await service.CreateLeaveRequestAsync(request, cancellationToken);
                return Results.Created($"/features/hr/leave-requests/{leaveRequest.Id}", leaveRequest);
            }
            catch (ArgumentException exception)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    [exception.ParamName ?? "request"] = [exception.Message]
                });
            }
        });

        group.MapPost("/leave-requests/{leaveRequestId:int}/decision", async (int leaveRequestId, DecideLeaveRequestRequest request, IHrService service, CancellationToken cancellationToken) =>
        {
            try
            {
                var leaveRequest = await service.DecideLeaveRequestAsync(leaveRequestId, request, cancellationToken);
                return leaveRequest is null ? Results.NotFound() : Results.Ok(leaveRequest);
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
