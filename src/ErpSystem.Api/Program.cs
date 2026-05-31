using System.Text;
using ErpSystem.Api.Features.Auth;
using ErpSystem.Api.Features.Contractors;
using ErpSystem.Api.Features.Dashboard;
using ErpSystem.Api.Features.Finance;
using ErpSystem.Api.Features.System;
using ErpSystem.Api.Features.HR;
using ErpSystem.Api.Features.Transport;
using ErpSystem.Api.Features.Warehouse;
using ErpSystem.Application.Modules.Auth;
using ErpSystem.Application.Modules.Contractors;
using ErpSystem.Application.Modules.Dashboard;
using ErpSystem.Application.Modules.Finance;
using ErpSystem.Application.Modules.HR;
using ErpSystem.Application.Modules.System.Foundation;
using ErpSystem.Application.Modules.Transport;
using ErpSystem.Application.Modules.Warehouse;
using ErpSystem.Infrastructure;
using ErpSystem.Infrastructure.Persistence;
using ErpSystem.Infrastructure.Security;
using ErpSystem.Shared.Modules;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ErpDb")
    ?? builder.Configuration.GetConnectionString("ErpSystemDatabase")
    ?? throw new InvalidOperationException("Connection string 'ErpDb' is missing.");
var jwtSection = builder.Configuration.GetSection(JwtOptions.SectionName);
var jwtOptions = jwtSection.Get<JwtOptions>() ?? throw new InvalidOperationException("JWT settings are missing.");
var jwtKey = jwtOptions.Key;
if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
{
    throw new InvalidOperationException("JWT key must contain at least 32 characters.");
}

builder.Services.AddProblemDetails();
builder.Services.AddAuthorization();
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddSingleton<IMenuBuilder, PermissionMenuBuilder>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISystemFoundationService, SystemFoundationService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IContractorService, ContractorService>();
builder.Services.AddScoped<IFinanceService, FinanceService>();
builder.Services.AddScoped<IHrService, HrService>();
builder.Services.AddScoped<ITransportService, TransportService>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddInfrastructure(
    connectionString,
    options => jwtSection.Bind(options),
    options => builder.Configuration.GetSection(BootstrapAdminOptions.SectionName).Bind(options));

var app = builder.Build();

await DatabaseBootstrapper.InitializeAsync(app.Services);

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Ok(new
{
    name = "ErpSystem.Api",
    modules = ErpModules.All.Select(module => module.Name).ToArray(),
    layers = new[]
    {
        nameof(ErpSystem.Domain.AssemblyMarker),
        nameof(ErpSystem.Application.AssemblyMarker),
        nameof(ErpSystem.Infrastructure.AssemblyMarker),
        nameof(ErpSystem.Shared.AssemblyMarker)
    }
}));

app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    environment = app.Environment.EnvironmentName,
    utc = DateTime.UtcNow
}));

app.MapAuthFeatures();
app.MapDashboardFeatures();
app.MapContractorsFeatures();
app.MapFinanceFeatures();
app.MapWarehouseFeatures();
app.MapTransportFeatures();
app.MapHrFeatures();
app.MapSystemFoundationFeatures();

app.Run();
