namespace ErpSystem.Application.Modules.System.Foundation;

public sealed record DictionaryItemView(
    int DictionaryItemId,
    string DictionaryName,
    string Code,
    string Name,
    string Value,
    int SortOrder,
    bool IsActive);
