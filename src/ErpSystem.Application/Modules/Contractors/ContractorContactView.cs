namespace ErpSystem.Application.Modules.Contractors;

public sealed record ContractorContactView(
    int Id,
    string FullName,
    string? Position,
    string? Email,
    string? PhoneNumber,
    bool IsPrimary);
