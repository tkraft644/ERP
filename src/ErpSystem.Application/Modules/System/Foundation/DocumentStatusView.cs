namespace ErpSystem.Application.Modules.System.Foundation;

public sealed record DocumentStatusView(
    int StatusId,
    string ModuleKey,
    string Code,
    string Name,
    int SortOrder,
    bool IsTerminal);
