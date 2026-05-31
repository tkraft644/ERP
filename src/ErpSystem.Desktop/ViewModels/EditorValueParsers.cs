using System.Globalization;

namespace ErpSystem.Desktop.ViewModels;

public static class EditorValueParsers
{
    public static DateTime ParseDate(string? value, string fieldName)
    {
        if (DateTime.TryParse(value, CultureInfo.GetCultureInfo("pl-PL"), DateTimeStyles.AssumeLocal, out var parsed))
        {
            return parsed;
        }

        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out parsed))
        {
            return parsed;
        }

        throw new InvalidOperationException($"{fieldName} ma niepoprawny format daty.");
    }

    public static decimal ParseDecimal(string? value, string fieldName)
    {
        if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.GetCultureInfo("pl-PL"), out var parsed))
        {
            return parsed;
        }

        if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out parsed))
        {
            return parsed;
        }

        throw new InvalidOperationException($"{fieldName} ma niepoprawny format liczby.");
    }

    public static int ParseInt(string? value, string fieldName)
    {
        if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
        {
            return parsed;
        }

        throw new InvalidOperationException($"{fieldName} ma niepoprawny format liczby całkowitej.");
    }
}
