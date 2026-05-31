using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ErpSystem.Desktop.ViewModels.Transport;

public sealed partial class TransportDispatchWorkspaceViewModel : WorkspaceViewModelBase
{
    private readonly List<TransportDispatchRowViewModel> allDispatchItems;

    public TransportDispatchWorkspaceViewModel()
        : base(
            "/transport/dispatch",
            "Transport",
            "Dispatch",
            "Planowanie obsady kierowcow, pojazdow i naczep dla aktywnych zlecen transportowych.",
            false)
    {
        allDispatchItems =
        [
            new TransportDispatchRowViewModel(
                "TR/202605/0042",
                "Zagraniczne",
                "PL -> DE -> NL",
                "15.05 06:30",
                "SCANIA R450",
                "KRONE 521",
                "Piotr Lewandowski",
                "Gotowe do wyjazdu",
                "#1F8A5B",
                "CMR gotowe",
                "96%",
                "#1F8A5B",
                false,
                false,
                false,
                "Nordic Steel",
                "Dyspozytor A",
                "Pojazd po serwisie, komplet dokumentow wydany kierowcy.",
                "SCANIA R460",
                "KRONE 544",
                "Marek Wysocki",
                [
                    new TransportDispatchTaskViewModel("Kontrola CMR", "Gotowe", "#1F8A5B"),
                    new TransportDispatchTaskViewModel("Kontakt z kierowca", "Potwierdzony", "#1F8A5B"),
                    new TransportDispatchTaskViewModel("Awizacja rozladunku", "Wyslana", "#2563EB")
                ]),
            new TransportDispatchRowViewModel(
                "TR/202605/0048",
                "Krajowe",
                "Lodz -> Poznan",
                "15.05 08:00",
                "Brak pojazdu",
                "Brak naczepy",
                "Brak kierowcy",
                "Do obsady",
                "#D14343",
                "Dokumenty robocze",
                "42%",
                "#D97706",
                true,
                true,
                false,
                "Ceramik SA",
                "Dyspozytor C",
                "Brakuje kompletu zasobow do uruchomienia trasy.",
                "VOLVO FH 500",
                "WIELTON 220",
                "Tomasz Krupa",
                [
                    new TransportDispatchTaskViewModel("Pojazd", "Brak", "#D14343"),
                    new TransportDispatchTaskViewModel("Kierowca", "Brak", "#D14343"),
                    new TransportDispatchTaskViewModel("Potwierdzenie klienta", "W toku", "#D97706")
                ]),
            new TransportDispatchRowViewModel(
                "TR/202605/0051",
                "Zagraniczne",
                "PL -> CZ",
                "15.05 05:40",
                "MAN TGX 18.510",
                "SCHMITZ S01",
                "Lukasz Wrobel",
                "W trasie",
                "#2563EB",
                "CMR w kabinie",
                "100%",
                "#1F8A5B",
                false,
                false,
                false,
                "Agro Trade",
                "Dyspozytor B",
                "Obsada i dokumenty potwierdzone, kierowca raportuje po zaladunku.",
                "MAN TGX 18.500",
                "SCHMITZ S04",
                "Rafal Kania",
                [
                    new TransportDispatchTaskViewModel("Check-in kierowcy", "Po zaladunku", "#2563EB"),
                    new TransportDispatchTaskViewModel("Dokumenty", "Gotowe", "#1F8A5B")
                ]),
            new TransportDispatchRowViewModel(
                "TR/202605/0054",
                "Krajowe",
                "Warszawa -> Gdansk",
                "15.05 12:30",
                "VOLVO FH 460",
                "WIELTON 401",
                "Anna Kaczmarek",
                "Dokumenty do potwierdzenia",
                "#D97706",
                "Brak podpisu klienta",
                "78%",
                "#D97706",
                false,
                false,
                true,
                "Market Fresh",
                "Dyspozytor D",
                "Obsada gotowa, ale instrukcja wydania nie ma jeszcze finalnej akceptacji.",
                "VOLVO FH 480",
                "WIELTON 410",
                "Anna Kaczmarek",
                [
                    new TransportDispatchTaskViewModel("Dokument klienta", "Brak potwierdzenia", "#D97706"),
                    new TransportDispatchTaskViewModel("Awizacja", "Wyslana", "#2563EB")
                ])
        ];

        SummaryCards = new ObservableCollection<TransportSummaryCardViewModel>();
        VisibleDispatchItems = new ObservableCollection<TransportDispatchRowViewModel>();
        StatusFilterOptions =
        [
            "Wszystkie statusy",
            "Do obsady",
            "Gotowe do wyjazdu",
            "W trasie",
            "Dokumenty do potwierdzenia"
        ];

        RunDispatchActionCommand = new RelayCommand<string>(RunDispatchAction);
        SelectDispatchCommand = new RelayCommand<TransportDispatchRowViewModel>(SelectDispatch);

        RefreshSummaryCards();
        ApplyFilters("TR/202605/0042");
    }

    public ObservableCollection<TransportSummaryCardViewModel> SummaryCards { get; }
    public ObservableCollection<TransportDispatchRowViewModel> VisibleDispatchItems { get; }
    public IReadOnlyList<string> StatusFilterOptions { get; }
    public IRelayCommand<string> RunDispatchActionCommand { get; }
    public IRelayCommand<TransportDispatchRowViewModel> SelectDispatchCommand { get; }

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private string selectedStatusFilter = "Wszystkie statusy";

    [ObservableProperty]
    private bool onlyUnassigned;

    [ObservableProperty]
    private bool onlyIssues;

    [ObservableProperty]
    private string lastActionMessage = "Wybierz zlecenie w dispatchu, aby przypisac zasoby i potwierdzic gotowosc.";

    [ObservableProperty]
    private TransportDispatchRowViewModel? selectedDispatchItem;

    public bool HasSelectedDispatchItem => SelectedDispatchItem is not null;
    public string SelectedDispatchNumber => SelectedDispatchItem?.Number ?? "-";
    public string SelectedDispatchType => SelectedDispatchItem?.OrderType ?? "-";
    public string SelectedDispatchRoute => SelectedDispatchItem?.RouteSummary ?? "-";
    public string SelectedDispatchCustomer => SelectedDispatchItem?.Customer ?? "-";
    public string SelectedDispatchDeparture => SelectedDispatchItem?.DepartureWindow ?? "-";
    public string SelectedDispatchVehicle => SelectedDispatchItem?.Vehicle ?? "-";
    public string SelectedDispatchTrailer => SelectedDispatchItem?.Trailer ?? "-";
    public string SelectedDispatchDriver => SelectedDispatchItem?.Driver ?? "-";
    public string SelectedDispatchStatus => SelectedDispatchItem?.Status ?? "-";
    public string SelectedDispatchStatusColor => SelectedDispatchItem?.StatusColor ?? "#7B8794";
    public string SelectedDispatchDocuments => SelectedDispatchItem?.DocumentState ?? "-";
    public string SelectedDispatchReadiness => SelectedDispatchItem?.ReadinessLabel ?? "-";
    public string SelectedDispatchReadinessColor => SelectedDispatchItem?.ReadinessColor ?? "#7B8794";
    public string SelectedDispatchSuggestedVehicle => SelectedDispatchItem?.SuggestedVehicle ?? "-";
    public string SelectedDispatchSuggestedTrailer => SelectedDispatchItem?.SuggestedTrailer ?? "-";
    public string SelectedDispatchSuggestedDriver => SelectedDispatchItem?.SuggestedDriver ?? "-";
    public string SelectedDispatchCoordinator => SelectedDispatchItem?.Coordinator ?? "-";
    public string SelectedDispatchNote => SelectedDispatchItem?.Note ?? "Brak notatki dispatch.";
    public IReadOnlyList<TransportDispatchTaskViewModel> SelectedDispatchTasks => SelectedDispatchItem?.Tasks ?? [];

    partial void OnSearchTextChanged(string value) => ApplyFilters();

    partial void OnSelectedStatusFilterChanged(string value) => ApplyFilters();

    partial void OnOnlyUnassignedChanged(bool value) => ApplyFilters();

    partial void OnOnlyIssuesChanged(bool value) => ApplyFilters();

    partial void OnSelectedDispatchItemChanged(TransportDispatchRowViewModel? value)
    {
        OnPropertyChanged(nameof(HasSelectedDispatchItem));
        OnPropertyChanged(nameof(SelectedDispatchNumber));
        OnPropertyChanged(nameof(SelectedDispatchType));
        OnPropertyChanged(nameof(SelectedDispatchRoute));
        OnPropertyChanged(nameof(SelectedDispatchCustomer));
        OnPropertyChanged(nameof(SelectedDispatchDeparture));
        OnPropertyChanged(nameof(SelectedDispatchVehicle));
        OnPropertyChanged(nameof(SelectedDispatchTrailer));
        OnPropertyChanged(nameof(SelectedDispatchDriver));
        OnPropertyChanged(nameof(SelectedDispatchStatus));
        OnPropertyChanged(nameof(SelectedDispatchStatusColor));
        OnPropertyChanged(nameof(SelectedDispatchDocuments));
        OnPropertyChanged(nameof(SelectedDispatchReadiness));
        OnPropertyChanged(nameof(SelectedDispatchReadinessColor));
        OnPropertyChanged(nameof(SelectedDispatchSuggestedVehicle));
        OnPropertyChanged(nameof(SelectedDispatchSuggestedTrailer));
        OnPropertyChanged(nameof(SelectedDispatchSuggestedDriver));
        OnPropertyChanged(nameof(SelectedDispatchCoordinator));
        OnPropertyChanged(nameof(SelectedDispatchNote));
        OnPropertyChanged(nameof(SelectedDispatchTasks));
    }

    private void ApplyFilters(string? preferredNumber = null)
    {
        var filtered = allDispatchItems
            .Where(item =>
                (string.IsNullOrWhiteSpace(SearchText) ||
                 item.Number.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 item.Customer.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 item.RouteSummary.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) &&
                (SelectedStatusFilter == "Wszystkie statusy" ||
                 string.Equals(item.Status, SelectedStatusFilter, StringComparison.OrdinalIgnoreCase)) &&
                (!OnlyUnassigned || item.NeedsVehicle || item.NeedsDriver) &&
                (!OnlyIssues || item.NeedsDocuments || item.NeedsVehicle || item.NeedsDriver))
            .ToArray();

        VisibleDispatchItems.Clear();
        foreach (var item in filtered)
        {
            VisibleDispatchItems.Add(item);
        }

        SelectedDispatchItem = VisibleDispatchItems.FirstOrDefault(item => item.Number == preferredNumber)
            ?? VisibleDispatchItems.FirstOrDefault(item => item.Number == SelectedDispatchItem?.Number)
            ?? VisibleDispatchItems.FirstOrDefault();

        RefreshSummaryCards();
    }

    private void RefreshSummaryCards()
    {
        var departures = allDispatchItems.Count;
        var missingDriver = allDispatchItems.Count(item => item.NeedsDriver);
        var missingVehicle = allDispatchItems.Count(item => item.NeedsVehicle);
        var docIssues = allDispatchItems.Count(item => item.NeedsDocuments);
        var ready = allDispatchItems.Count(item => string.Equals(item.Status, "Gotowe do wyjazdu", StringComparison.OrdinalIgnoreCase));

        SummaryCards.Clear();
        SummaryCards.Add(new TransportSummaryCardViewModel("Wyjazdy dzisiaj", departures.ToString(), "Aktywne zlecenia widoczne w planie dispatch.", "#2563EB"));
        SummaryCards.Add(new TransportSummaryCardViewModel("Brak kierowcy", missingDriver.ToString(), "Zlecenia czekajace na potwierdzenie kierowcy.", "#D14343"));
        SummaryCards.Add(new TransportSummaryCardViewModel("Brak pojazdu", missingVehicle.ToString(), "Trasy bez przypisanego ciagnika albo naczepy.", "#D97706"));
        SummaryCards.Add(new TransportSummaryCardViewModel("Dokumenty", docIssues.ToString(), "Pakiety dokumentow wymagajace potwierdzenia przed wyjazdem.", "#D97706"));
        SummaryCards.Add(new TransportSummaryCardViewModel("Gotowe", ready.ToString(), "Dispatchy gotowe do wydania kierowcy.", "#1F8A5B"));
    }

    private void SelectDispatch(TransportDispatchRowViewModel? item)
    {
        if (item is null)
        {
            return;
        }

        SelectedDispatchItem = item;
    }

    private void RunDispatchAction(string? action)
    {
        if (SelectedDispatchItem is null || string.IsNullOrWhiteSpace(action))
        {
            return;
        }

        switch (action)
        {
            case "Pojazd":
                AssignVehicleAndTrailer();
                break;
            case "Kierowca":
                AssignDriver();
                break;
            case "Potwierdz":
                ConfirmDispatch();
                break;
            case "Eksport":
                LastActionMessage = $"Wyeksportowano plan dispatch dla {SelectedDispatchItem.Number}.";
                break;
        }
    }

    private void AssignVehicleAndTrailer()
    {
        if (SelectedDispatchItem is null)
        {
            return;
        }

        ReplaceDispatch(SelectedDispatchItem with
        {
            Vehicle = SelectedDispatchItem.SuggestedVehicle,
            Trailer = SelectedDispatchItem.SuggestedTrailer,
            NeedsVehicle = false,
            Status = SelectedDispatchItem.NeedsDriver ? "Do obsady" : "Dokumenty do potwierdzenia",
            StatusColor = SelectedDispatchItem.NeedsDriver ? "#D14343" : "#D97706",
            ReadinessLabel = SelectedDispatchItem.NeedsDriver ? "68%" : "82%",
            ReadinessColor = "#D97706"
        });

        LastActionMessage = $"Przypisano pojazd {SelectedDispatchItem.Vehicle} i naczepę {SelectedDispatchItem.Trailer}.";
    }

    private void AssignDriver()
    {
        if (SelectedDispatchItem is null)
        {
            return;
        }

        ReplaceDispatch(SelectedDispatchItem with
        {
            Driver = SelectedDispatchItem.SuggestedDriver,
            NeedsDriver = false,
            Status = SelectedDispatchItem.NeedsVehicle ? "Do obsady" : "Dokumenty do potwierdzenia",
            StatusColor = SelectedDispatchItem.NeedsVehicle ? "#D14343" : "#D97706",
            ReadinessLabel = SelectedDispatchItem.NeedsVehicle ? "68%" : "82%",
            ReadinessColor = "#D97706"
        });

        LastActionMessage = $"Przypisano kierowcę {SelectedDispatchItem.Driver} do zlecenia {SelectedDispatchItem.Number}.";
    }

    private void ConfirmDispatch()
    {
        if (SelectedDispatchItem is null)
        {
            return;
        }

        ReplaceDispatch(SelectedDispatchItem with
        {
            NeedsDriver = false,
            NeedsVehicle = false,
            NeedsDocuments = false,
            Status = "Gotowe do wyjazdu",
            StatusColor = "#1F8A5B",
            DocumentState = "Komplet wydany",
            ReadinessLabel = "100%",
            ReadinessColor = "#1F8A5B"
        });

        LastActionMessage = $"Potwierdzono dispatch i gotowosc wyjazdu dla {SelectedDispatchItem.Number}.";
    }

    private void ReplaceDispatch(TransportDispatchRowViewModel updated)
    {
        var index = allDispatchItems.FindIndex(item => item.Number == updated.Number);
        if (index < 0)
        {
            return;
        }

        allDispatchItems[index] = updated;
        ApplyFilters(updated.Number);
    }
}

public sealed record TransportDispatchRowViewModel(
    string Number,
    string OrderType,
    string RouteSummary,
    string DepartureWindow,
    string Vehicle,
    string Trailer,
    string Driver,
    string Status,
    string StatusColor,
    string DocumentState,
    string ReadinessLabel,
    string ReadinessColor,
    bool NeedsVehicle,
    bool NeedsDriver,
    bool NeedsDocuments,
    string Customer,
    string Coordinator,
    string Note,
    string SuggestedVehicle,
    string SuggestedTrailer,
    string SuggestedDriver,
    IReadOnlyList<TransportDispatchTaskViewModel> Tasks);

public sealed record TransportDispatchTaskViewModel(
    string Name,
    string State,
    string StateColor);
