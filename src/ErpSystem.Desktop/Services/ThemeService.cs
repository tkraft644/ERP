using Avalonia;
using Avalonia.Styling;

namespace ErpSystem.Desktop.Services;

public sealed class ThemeService
{
    public IReadOnlyList<string> Options { get; } = ["System", "Jasny", "Ciemny"];

    public string CurrentMode
    {
        get
        {
            var variant = Application.Current?.RequestedThemeVariant;

            if (variant == ThemeVariant.Light)
            {
                return "Jasny";
            }

            if (variant == ThemeVariant.Dark)
            {
                return "Ciemny";
            }

            return "System";
        }
    }

    public void Apply(string? mode)
    {
        if (Application.Current is null)
        {
            return;
        }

        Application.Current.RequestedThemeVariant = mode switch
        {
            "Jasny" => ThemeVariant.Light,
            "Ciemny" => ThemeVariant.Dark,
            _ => ThemeVariant.Default
        };
    }
}
