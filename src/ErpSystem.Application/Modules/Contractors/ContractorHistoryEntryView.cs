namespace ErpSystem.Application.Modules.Contractors;

public sealed record ContractorHistoryEntryView(
    int Id,
    string ActionName,
    int? ChangedByUserId,
    DateTime ChangedAtUtc,
    string Summary,
    string OldValues,
    string NewValues);
