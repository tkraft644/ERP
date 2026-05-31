namespace ErpSystem.Desktop.ViewModels.Transport;

public sealed class TransportOrderEditorWorkspaceViewModel : ModuleWorkspaceViewModelBase
{
    private const string Accent = "#1D4ED8";
    private const string Surface = "#EDF3FF";

    public TransportOrderEditorWorkspaceViewModel()
        : base(
            "/transport/orders/edit",
            "Transport",
            "Edycja zlecenia",
            "Osobny workspace dla konkretnego zlecenia z sekcjami trasy, stopow i dokumentow.",
            true,
            "Edycja zlecenia nie powinna nadpisywac listy. Operator ma miec mozliwosc rownoleglego porownania kilku zlecen w osobnych zakladkach.",
            "Editor",
            Accent,
            Surface,
            [
                new WorkspaceMetricViewModel("Numer", "TR/202605/0042", "Przykladowe zlecenie w edycji.", Accent),
                new WorkspaceMetricViewModel("Status", "Zaplanowane", "Gotowe do dalszej obsady i potwierdzen.", Accent),
                new WorkspaceMetricViewModel("Stops", "4", "Dwa punkty zaladunku i dwa rozladunku.", Accent)
            ],
            BuildActions("Glowne", Accent, Surface,
                ("Glowne", "Dane klienta, typ przewozu i status zlecenia."),
                ("Trasa i stop'y", "Sekwencja punktow, czasy i kraje."),
                ("Dokumenty i koszty", "CMR, odprawy oraz powiazane koszty.")),
            [
                new WorkspaceActivityViewModel("Multi-edit", "Ten ekran jest closowalny i przygotowany pod wiele otwartych rekordow.", "Gotowe", Accent),
                new WorkspaceActivityViewModel("Driver profile", "Podlacz wybor pracownika z profilem kierowcy z HR.", "Plan", "#F59E0B")
            ])
    {
    }
}
