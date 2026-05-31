using System.Globalization;

namespace ErpSystem.Desktop.ViewModels.HR;

internal static class HrEditorParsers
{
    public static DateTime ParseRequiredDate(string? value, string fieldName)
    {
        if (TryParseDate(value, out var parsed))
        {
            return parsed!.Value;
        }

        throw new InvalidOperationException($"Pole {fieldName} musi zawierać prawidłową datę.");
    }

    public static DateTime? ParseOptionalDate(string? value)
        => TryParseDate(value, out var parsed) ? parsed : null;

    public static decimal ParseRequiredDecimal(string? value, string fieldName)
    {
        if (TryParseDecimal(value, out var parsed))
        {
            return parsed;
        }

        throw new InvalidOperationException($"Pole {fieldName} musi zawierać prawidłową liczbę.");
    }

    public static decimal? ParseOptionalDecimal(string? value)
        => TryParseDecimal(value, out var parsed) ? parsed : null;

    private static bool TryParseDate(string? value, out DateTime? parsed)
    {
        parsed = null;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        string[] formats = ["dd.MM.yyyy", "yyyy-MM-dd", "dd/MM/yyyy"];
        if (DateTime.TryParseExact(value.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var exactDate) ||
            DateTime.TryParse(value.Trim(), CultureInfo.CurrentCulture, DateTimeStyles.None, out exactDate))
        {
            parsed = exactDate.Date;
            return true;
        }

        return false;
    }

    private static bool TryParseDecimal(string? value, out decimal parsed)
    {
        parsed = 0m;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        return decimal.TryParse(value.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out parsed) ||
               decimal.TryParse(value.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out parsed);
    }
}
