namespace ErpSystem.Application.Modules.System.Foundation;

public sealed record DocumentNumberSequenceView(
    int SequenceId,
    string Key,
    string Prefix,
    int CurrentNumber,
    int Padding,
    string ResetPolicy);
