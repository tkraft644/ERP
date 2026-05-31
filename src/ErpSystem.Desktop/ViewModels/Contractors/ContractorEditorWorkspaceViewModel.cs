namespace ErpSystem.Desktop.ViewModels.Contractors;

public sealed class ContractorEditorWorkspaceViewModel : ModuleWorkspaceViewModelBase
{
    private const string Accent = "#1D4ED8";
    private const string Surface = "#EAF1FF";

    public ContractorEditorWorkspaceViewModel()
        : base(
            "/contractors/edit",
            "Kontrahenci",
            "Edycja kontrahenta",
            "Osobny workspace edycyjny z formularzem, notatkami i danymi rozliczeniowymi.",
            true,
            "Workspace edycji powinien byc niezalezny od listy, tak zeby uzytkownik mogl miec otwartych kilka rekordow i wracac do nich zakladkami.",
            "Editor",
            Accent,
            Surface,
            [
                new WorkspaceMetricViewModel("Podmiot", "Baltic Retail", "Przykladowy rekord aktywny w kartotece.", Accent),
                new WorkspaceMetricViewModel("Typy", "Klient / Odbiorca", "Jeden kontrahent moze pelnic kilka rol operacyjnych.", Accent),
                new WorkspaceMetricViewModel("Zmiany oczekujace", "3", "Adres, konto bankowe i limit kredytowy do zatwierdzenia.", Accent)
            ],
            BuildActions("Dane glowne", Accent, Surface,
                ("Dane glowne", "Nazwa, NIP, status i typy kontrahenta."),
                ("Adresy logistyczne", "Miejsca zaladunku, rozladunku i adres glownego biura."),
                ("Finanse", "Konta bankowe, warunki platnosci i notatki.")),
            [
                new WorkspaceActivityViewModel("Tryb wielozakladkowy", "Ten ekran jest closowalny i powinien otwierac sie obok listy.", "Gotowe", Accent),
                new WorkspaceActivityViewModel("Walidacja", "Nastepnym krokiem bedzie podpinka do API i formularza CRUD.", "Plan", "#F59E0B")
            ])
    {
    }
}
