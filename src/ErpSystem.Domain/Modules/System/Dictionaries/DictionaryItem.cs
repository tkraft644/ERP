namespace ErpSystem.Domain.Modules.System.Dictionaries;

public sealed class DictionaryItem : Common.AuditableEntity
{
    public DictionaryItem(
        string dictionaryName,
        string code,
        string name,
        string value,
        int sortOrder,
        bool isActive = true)
    {
        DictionaryName = dictionaryName;
        Code = code;
        Name = name;
        Value = value;
        SortOrder = sortOrder;
        IsActive = isActive;
    }

    public string DictionaryName { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
}
