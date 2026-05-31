namespace ErpSystem.Application.Modules.System.Foundation;

public sealed record DocumentHistoryView(
    int EntryId,
    string DocumentType,
    int DocumentId,
    string EntryType,
    string Description,
    string? StatusCode,
    DateTime OccurredAtUtc);
