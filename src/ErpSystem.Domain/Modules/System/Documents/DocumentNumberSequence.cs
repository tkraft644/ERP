namespace ErpSystem.Domain.Modules.System.Documents;

public sealed class DocumentNumberSequence : Common.AuditableEntity
{
    private DocumentNumberSequence()
    {
    }

    public DocumentNumberSequence(
        string key,
        string prefix,
        int currentNumber,
        int padding,
        DocumentNumberResetPolicy resetPolicy)
        : this(key, prefix, currentNumber, padding, resetPolicy, DateTime.UtcNow)
    {
    }

    public DocumentNumberSequence(
        string key,
        string prefix,
        int currentNumber,
        int padding,
        DocumentNumberResetPolicy resetPolicy,
        DateTime lastGeneratedAtUtc)
    {
        Key = key;
        Prefix = prefix;
        CurrentNumber = currentNumber;
        Padding = padding;
        ResetPolicy = resetPolicy;
        LastGeneratedAtUtc = lastGeneratedAtUtc;
    }

    public string Key { get; set; } = string.Empty;
    public string Prefix { get; set; } = string.Empty;
    public int CurrentNumber { get; private set; }
    public int Padding { get; set; }
    public DocumentNumberResetPolicy ResetPolicy { get; set; }
    public DateTime LastGeneratedAtUtc { get; private set; }

    public string GenerateNextNumber(DateTime utcNow)
    {
        if (ShouldResetCounter(utcNow))
        {
            CurrentNumber = 0;
        }

        CurrentNumber++;
        LastGeneratedAtUtc = utcNow;

        return ResetPolicy switch
        {
            DocumentNumberResetPolicy.Yearly => $"{Prefix}/{utcNow:yyyy}/{CurrentNumber.ToString().PadLeft(Padding, '0')}",
            DocumentNumberResetPolicy.Monthly => $"{Prefix}/{utcNow:yyyyMM}/{CurrentNumber.ToString().PadLeft(Padding, '0')}",
            _ => $"{Prefix}/{CurrentNumber.ToString().PadLeft(Padding, '0')}"
        };
    }

    private bool ShouldResetCounter(DateTime utcNow)
    {
        return ResetPolicy switch
        {
            DocumentNumberResetPolicy.Yearly => LastGeneratedAtUtc.Year != utcNow.Year,
            DocumentNumberResetPolicy.Monthly => LastGeneratedAtUtc.Year != utcNow.Year || LastGeneratedAtUtc.Month != utcNow.Month,
            _ => false
        };
    }
}
