using ErpSystem.Domain.Modules.System.Documents;

namespace ErpSystem.Tests;

public class DocumentNumberSequenceTests
{
    [Fact]
    public void GenerateNextNumber_ShouldIncrementAndFormatYearlySequence()
    {
        var sequence = new DocumentNumberSequence(
            "WAREHOUSE_GOODS",
            "WG",
            12,
            5,
            DocumentNumberResetPolicy.Yearly);

        var value = sequence.GenerateNextNumber(new DateTime(2026, 5, 14, 10, 0, 0, DateTimeKind.Utc));

        Assert.Equal("WG/2026/00013", value);
    }

    [Fact]
    public void GenerateNextNumber_ShouldResetMonthlySequenceWhenMonthChanges()
    {
        var sequence = new DocumentNumberSequence(
            "TR_ORD",
            "TR",
            9,
            4,
            DocumentNumberResetPolicy.Monthly);

        sequence.GenerateNextNumber(new DateTime(2026, 4, 30, 8, 0, 0, DateTimeKind.Utc));
        var next = sequence.GenerateNextNumber(new DateTime(2026, 5, 1, 8, 0, 0, DateTimeKind.Utc));

        Assert.Equal("TR/202605/0001", next);
    }
}
