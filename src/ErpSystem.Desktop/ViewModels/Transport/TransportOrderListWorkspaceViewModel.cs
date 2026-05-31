using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ErpSystem.Desktop.ViewModels.Transport;

public sealed partial class TransportOrderListWorkspaceViewModel : WorkspaceViewModelBase
{
    private readonly List<TransportOrderRowViewModel> allOrders;

    public TransportOrderListWorkspaceViewModel()
        : base(
            "/transport/orders",
            "Transport",
            "Lista zlecen",
            "Wspolna lista zlecen krajowych i zagranicznych z obsluga statusow, obsady i dokumentow.",
            false)
    {
        allOrders =
        [
            new TransportOrderRowViewModel(
                "TR/202605/0042",
                "Zagraniczne",
                "Zaplanowane",
                "#2563EB",
                "Nordic Steel",
                "PL -> DE -> NL",
                "15.05 06:30",
                "16.05 11:15",
                "SCANIA R450",
                "Piotr Lewandowski",
                "12 800 EUR",
                false,
                false,
                false,
                "Baltic Carrier",
                "KRONE 521",
                "DAP",
                "EUR",
                "4.31",
                "CMR gotowe, odprawa bez uwag",
                "Niemcy / Benelux",
                "FTL eksport",
                "Ladunek hutniczy z dwoma punktami rozladunku.",
                [
                    new TransportStopLineViewModel("Zaladunek", "Gliwice", "PL", "15.05 06:30"),
                    new TransportStopLineViewModel("Rozladunek", "Berlin", "DE", "15.05 16:20"),
                    new TransportStopLineViewModel("Rozladunek", "Rotterdam", "NL", "16.05 11:15")
                ],
                [
                    new TransportDocumentLineViewModel("CMR", "Gotowe", "#1F8A5B"),
                    new TransportDocumentLineViewModel("Instrukcja zaladunku", "Wyslana", "#2563EB"),
                    new TransportDocumentLineViewModel("Odprawa celna", "Nie dotyczy", "#7B8794")
                ],
                [
                    new TransportStatusHistoryLineViewModel("14.05 15:20", "Przyjeto", "Dyspozytor A"),
                    new TransportStatusHistoryLineViewModel("14.05 16:05", "Zaplanowane", "Dyspozytor A")
                ]),
            new TransportOrderRowViewModel(
                "TR/202605/0048",
                "Krajowe",
                "Nowe",
                "#2563EB",
                "Ceramik SA",
                "Lodz -> Poznan",
                "15.05 08:00",
                "15.05 14:30",
                "Brak pojazdu",
                "Brak kierowcy",
                "4 900 PLN",
                false,
                true,
                true,
                "Fleet One",
                "Brak naczepy",
                "-",
                "PLN",
                "1.00",
                "Brak kompletu obsady do wyjazdu.",
                "Wielkopolska",
                "Dystrybucja krajowa",
                "Zlecenie czeka na przypisanie pojazdu i kierowcy.",
                [
                    new TransportStopLineViewModel("Zaladunek", "Lodz", "PL", "15.05 08:00"),
                    new TransportStopLineViewModel("Rozladunek", "Poznan", "PL", "15.05 14:30")
                ],
                [
                    new TransportDocumentLineViewModel("List przewozowy", "Do przygotowania", "#D97706"),
                    new TransportDocumentLineViewModel("Potwierdzenie klienta", "Brak", "#D14343")
                ],
                [
                    new TransportStatusHistoryLineViewModel("14.05 17:10", "Nowe", "System")
                ]),
            new TransportOrderRowViewModel(
                "TR/202605/0051",
                "Zagraniczne",
                "W realizacji",
                "#1F8A5B",
                "Agro Trade",
                "PL -> CZ",
                "15.05 05:40",
                "15.05 18:00",
                "MAN TGX 18.510",
                "Lukasz Wrobel",
                "7 600 EUR",
                false,
                false,
                false,
                "EuroWays",
                "SCHMITZ S01",
                "FCA",
                "EUR",
                "4.31",
                "Dokumenty w kabinie, crossing zatwierdzony.",
                "Czechy",
                "FTL miedzynarodowy",
                "Zlecenie jedzie zgodnie z planem, bez alertow.",
                [
                    new TransportStopLineViewModel("Zaladunek", "Katowice", "PL", "15.05 05:40"),
                    new TransportStopLineViewModel("Rozladunek", "Brno", "CZ", "15.05 18:00")
                ],
                [
                    new TransportDocumentLineViewModel("CMR", "W trasie", "#1F8A5B"),
                    new TransportDocumentLineViewModel("Awizacja", "Potwierdzona", "#2563EB")
                ],
                [
                    new TransportStatusHistoryLineViewModel("14.05 13:40", "Zaplanowane", "Dyspozytor B"),
                    new TransportStatusHistoryLineViewModel("15.05 05:42", "W realizacji", "Kierowca")
                ]),
            new TransportOrderRowViewModel(
                "TR/202605/0054",
                "Krajowe",
                "Przyjete",
                "#D97706",
                "Market Fresh",
                "Warszawa -> Gdansk",
                "15.05 12:30",
                "15.05 19:45",
                "VOLVO FH 460",
                "Anna Kaczmarek",
                "5 300 PLN",
                false,
                false,
                true,
                "North Cargo",
                "WIELTON 401",
                "-",
                "PLN",
                "1.00",
                "Brakuje podpisanej instrukcji wydania na chlodnie.",
                "Pomorskie",
                "Chlodnia krajowa",
                "Obsada gotowa, ale dokumenty klienta wymagaja potwierdzenia.",
                [
                    new TransportStopLineViewModel("Zaladunek", "Warszawa", "PL", "15.05 12:30"),
                    new TransportStopLineViewModel("Rozladunek", "Gdansk", "PL", "15.05 19:45")
                ],
                [
                    new TransportDocumentLineViewModel("Instrukcja wydania", "Do potwierdzenia", "#D97706"),
                    new TransportDocumentLineViewModel("Awizacja", "Wyslana", "#2563EB")
                ],
                [
                    new TransportStatusHistoryLineViewModel("14.05 18:10", "Przyjete", "Dyspozytor C")
                ]),
            new TransportOrderRowViewModel(
                "TR/202605/0031",
                "Zagraniczne",
                "Zamkniete",
                "#7B8794",
                "Inter Parts",
                "PL -> FR",
                "13.05 04:20",
                "14.05 09:10",
                "RENAULT T",
                "Michal Wojcik",
                "14 200 EUR",
                true,
                false,
                false,
                "West Logistics",
                "KRONE 314",
                "CPT",
                "EUR",
                "4.29",
                "Dokumenty rozliczone, zlecenie zamkniete.",
                "Francja",
                "FTL miedzynarodowy",
                "Zakonczone i gotowe do archiwum.",
                [
                    new TransportStopLineViewModel("Zaladunek", "Wroclaw", "PL", "13.05 04:20"),
                    new TransportStopLineViewModel("Rozladunek", "Lyon", "FR", "14.05 09:10")
                ],
                [
                    new TransportDocumentLineViewModel("CMR", "Rozliczone", "#1F8A5B"),
                    new TransportDocumentLineViewModel("Potwierdzenie dostawy", "Odebrane", "#1F8A5B")
                ],
                [
                    new TransportStatusHistoryLineViewModel("14.05 09:15", "Dostarczone", "Kierowca"),
                    new TransportStatusHistoryLineViewModel("14.05 11:00", "Zamkniete", "Koordynator")
                ])
        ];

        SummaryCards = new ObservableCollection<TransportSummaryCardViewModel>();
        VisibleOrders = new ObservableCollection<TransportOrderRowViewModel>();
        TypeFilterOptions = ["Wszystkie typy", "Krajowe", "Zagraniczne"];
        StatusFilterOptions =
        [
            "Wszystkie statusy",
            "Nowe",
            "Przyjete",
            "Zaplanowane",
            "W realizacji",
            "Załadowane",
            "Dostarczone",
            "Zamkniete"
        ];
        CarrierFilterOptions = ["Wszyscy przewoznicy", .. allOrders
            .Select(item => item.Carrier)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(item => item)];

        RunOrderActionCommand = new RelayCommand<string>(RunOrderAction);
        SelectOrderCommand = new RelayCommand<TransportOrderRowViewModel>(SelectOrder);

        RefreshSummaryCards();
        ApplyFilters("TR/202605/0042");
    }

    public ObservableCollection<TransportSummaryCardViewModel> SummaryCards { get; }
    public ObservableCollection<TransportOrderRowViewModel> VisibleOrders { get; }
    public IReadOnlyList<string> TypeFilterOptions { get; }
    public IReadOnlyList<string> StatusFilterOptions { get; }
    public IReadOnlyList<string> CarrierFilterOptions { get; }
    public IRelayCommand<string> RunOrderActionCommand { get; }
    public IRelayCommand<TransportOrderRowViewModel> SelectOrderCommand { get; }

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private string selectedTypeFilter = "Wszystkie typy";

    [ObservableProperty]
    private string selectedStatusFilter = "Wszystkie statusy";

    [ObservableProperty]
    private string selectedCarrierFilter = "Wszyscy przewoznicy";

    [ObservableProperty]
    private bool onlyUnassigned;

    [ObservableProperty]
    private bool onlyAttention;

    [ObservableProperty]
    private bool showArchived;

    [ObservableProperty]
    private string lastActionMessage = "Wybierz zlecenie, aby zobaczyc trase, dokumenty i obsluge dispatch.";

    [ObservableProperty]
    private TransportOrderRowViewModel? selectedOrder;

    public bool HasSelectedOrder => SelectedOrder is not null;
    public string SelectedOrderNumber => SelectedOrder?.Number ?? "-";
    public string SelectedOrderType => SelectedOrder?.OrderType ?? "-";
    public string SelectedOrderStatus => SelectedOrder?.Status ?? "-";
    public string SelectedOrderStatusColor => SelectedOrder?.StatusColor ?? "#7B8794";
    public string SelectedOrderCustomer => SelectedOrder?.Customer ?? "-";
    public string SelectedOrderRoute => SelectedOrder?.RouteSummary ?? "-";
    public string SelectedOrderLoading => SelectedOrder?.LoadingWindow ?? "-";
    public string SelectedOrderDelivery => SelectedOrder?.DeliveryWindow ?? "-";
    public string SelectedOrderVehicle => SelectedOrder?.Vehicle ?? "-";
    public string SelectedOrderDriver => SelectedOrder?.Driver ?? "-";
    public string SelectedOrderTrailer => SelectedOrder?.Trailer ?? "-";
    public string SelectedOrderCarrier => SelectedOrder?.Carrier ?? "-";
    public string SelectedOrderAmount => SelectedOrder?.AmountLabel ?? "-";
    public string SelectedOrderCompliance => SelectedOrder?.ComplianceSummary ?? "-";
    public string SelectedOrderMode => SelectedOrder?.ModeSummary ?? "-";
    public string SelectedOrderNote => SelectedOrder?.Note ?? "Brak notatki operacyjnej.";
    public IReadOnlyList<TransportStopLineViewModel> SelectedOrderStops => SelectedOrder?.Stops ?? [];
    public IReadOnlyList<TransportDocumentLineViewModel> SelectedOrderDocuments => SelectedOrder?.Documents ?? [];
    public IReadOnlyList<TransportStatusHistoryLineViewModel> SelectedOrderHistory => SelectedOrder?.History ?? [];

    partial void OnSearchTextChanged(string value) => ApplyFilters();

    partial void OnSelectedTypeFilterChanged(string value) => ApplyFilters();

    partial void OnSelectedStatusFilterChanged(string value) => ApplyFilters();

    partial void OnSelectedCarrierFilterChanged(string value) => ApplyFilters();

    partial void OnOnlyUnassignedChanged(bool value) => ApplyFilters();

    partial void OnOnlyAttentionChanged(bool value) => ApplyFilters();

    partial void OnShowArchivedChanged(bool value) => ApplyFilters();

    partial void OnSelectedOrderChanged(TransportOrderRowViewModel? value)
    {
        OnPropertyChanged(nameof(HasSelectedOrder));
        OnPropertyChanged(nameof(SelectedOrderNumber));
        OnPropertyChanged(nameof(SelectedOrderType));
        OnPropertyChanged(nameof(SelectedOrderStatus));
        OnPropertyChanged(nameof(SelectedOrderStatusColor));
        OnPropertyChanged(nameof(SelectedOrderCustomer));
        OnPropertyChanged(nameof(SelectedOrderRoute));
        OnPropertyChanged(nameof(SelectedOrderLoading));
        OnPropertyChanged(nameof(SelectedOrderDelivery));
        OnPropertyChanged(nameof(SelectedOrderVehicle));
        OnPropertyChanged(nameof(SelectedOrderDriver));
        OnPropertyChanged(nameof(SelectedOrderTrailer));
        OnPropertyChanged(nameof(SelectedOrderCarrier));
        OnPropertyChanged(nameof(SelectedOrderAmount));
        OnPropertyChanged(nameof(SelectedOrderCompliance));
        OnPropertyChanged(nameof(SelectedOrderMode));
        OnPropertyChanged(nameof(SelectedOrderNote));
        OnPropertyChanged(nameof(SelectedOrderStops));
        OnPropertyChanged(nameof(SelectedOrderDocuments));
        OnPropertyChanged(nameof(SelectedOrderHistory));
    }

    private void ApplyFilters(string? preferredNumber = null)
    {
        var filtered = allOrders
            .Where(item =>
                (string.IsNullOrWhiteSpace(SearchText) ||
                 item.Number.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 item.Customer.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                 item.RouteSummary.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) &&
                (SelectedTypeFilter == "Wszystkie typy" ||
                 string.Equals(item.OrderType, SelectedTypeFilter, StringComparison.OrdinalIgnoreCase)) &&
                (SelectedStatusFilter == "Wszystkie statusy" ||
                 string.Equals(item.Status, SelectedStatusFilter, StringComparison.OrdinalIgnoreCase)) &&
                (SelectedCarrierFilter == "Wszyscy przewoznicy" ||
                 string.Equals(item.Carrier, SelectedCarrierFilter, StringComparison.OrdinalIgnoreCase)) &&
                (!OnlyUnassigned || item.IsUnassigned) &&
                (!OnlyAttention || item.RequiresAttention) &&
                (ShowArchived || !item.IsArchived))
            .ToArray();

        VisibleOrders.Clear();
        foreach (var item in filtered)
        {
            VisibleOrders.Add(item);
        }

        SelectedOrder = VisibleOrders.FirstOrDefault(item => item.Number == preferredNumber)
            ?? VisibleOrders.FirstOrDefault(item => item.Number == SelectedOrder?.Number)
            ?? VisibleOrders.FirstOrDefault();

        RefreshSummaryCards();
    }

    private void RefreshSummaryCards()
    {
        IEnumerable<TransportOrderRowViewModel> source = ShowArchived
            ? allOrders
            : allOrders.Where(item => !item.IsArchived);

        var active = source.Count(item => !string.Equals(item.Status, "Zamkniete", StringComparison.OrdinalIgnoreCase));
        var unassigned = source.Count(item => item.IsUnassigned);
        var inProgress = source.Count(item => string.Equals(item.Status, "W realizacji", StringComparison.OrdinalIgnoreCase));
        var international = source.Count(item => string.Equals(item.OrderType, "Zagraniczne", StringComparison.OrdinalIgnoreCase));
        var attention = source.Count(item => item.RequiresAttention);

        SummaryCards.Clear();
        SummaryCards.Add(new TransportSummaryCardViewModel("Aktywne zlecenia", active.ToString(), "Zlecenia otwarte na dzisiejszej i nocnej zmianie.", "#2563EB"));
        SummaryCards.Add(new TransportSummaryCardViewModel("Do obsady", unassigned.ToString(), "Zlecenia bez pelnej obsady kierowca / pojazd / naczepa.", "#D14343"));
        SummaryCards.Add(new TransportSummaryCardViewModel("W realizacji", inProgress.ToString(), "Trasy, ktore sa juz w drodze lub po zaladunku.", "#1F8A5B"));
        SummaryCards.Add(new TransportSummaryCardViewModel("Zagraniczne", international.ToString(), "Zlecenia z waluta, incoterms lub wymaganiami granicznymi.", "#2563EB"));
        SummaryCards.Add(new TransportSummaryCardViewModel("Ryzyka operacyjne", attention.ToString(), "Obsada, dokumenty albo terminy wymagajace reakcji dyspozytora.", "#D97706"));
    }

    private void SelectOrder(TransportOrderRowViewModel? order)
    {
        if (order is null)
        {
            return;
        }

        SelectedOrder = order;
    }

    private void RunOrderAction(string? action)
    {
        if (SelectedOrder is null || string.IsNullOrWhiteSpace(action))
        {
            return;
        }

        switch (action)
        {
            case "Edytuj":
                LastActionMessage = $"Otwarto zlecenie {SelectedOrder.Number} do edycji operacyjnej.";
                break;
            case "Dispatch":
                LastActionMessage = $"Przekazano zlecenie {SelectedOrder.Number} do planowania dispatch.";
                break;
            case "Status":
                AdvanceSelectedStatus();
                break;
            case "Eksportuj":
                LastActionMessage = $"Wyeksportowano zestawienie zlecenia {SelectedOrder.Number} do XLSX i PDF.";
                break;
            case "Archiwizuj":
                ArchiveSelectedOrder();
                break;
        }
    }

    private void AdvanceSelectedStatus()
    {
        if (SelectedOrder is null)
        {
            return;
        }

        var (nextStatus, color) = SelectedOrder.Status switch
        {
            "Nowe" => ("Przyjete", "#D97706"),
            "Przyjete" => ("Zaplanowane", "#2563EB"),
            "Zaplanowane" => ("W realizacji", "#1F8A5B"),
            "W realizacji" => ("Załadowane", "#1F8A5B"),
            "Załadowane" => ("Dostarczone", "#1F8A5B"),
            "Dostarczone" => ("Zamkniete", "#7B8794"),
            _ => ("Zamkniete", "#7B8794")
        };

        ReplaceOrder(SelectedOrder with
        {
            Status = nextStatus,
            StatusColor = color,
            RequiresAttention = false
        });

        LastActionMessage = $"Zmieniono status zlecenia {SelectedOrder.Number} na {nextStatus}.";
    }

    private void ArchiveSelectedOrder()
    {
        if (SelectedOrder is null)
        {
            return;
        }

        if (SelectedOrder.IsArchived)
        {
            LastActionMessage = $"Zlecenie {SelectedOrder.Number} jest juz w archiwum.";
            return;
        }

        ReplaceOrder(SelectedOrder with
        {
            IsArchived = true,
            Status = "Zamkniete",
            StatusColor = "#7B8794",
            RequiresAttention = false
        });

        LastActionMessage = $"Zlecenie {SelectedOrder.Number} przeniesiono do archiwum transportu.";
    }

    private void ReplaceOrder(TransportOrderRowViewModel updated)
    {
        var index = allOrders.FindIndex(item => item.Number == updated.Number);
        if (index < 0)
        {
            return;
        }

        allOrders[index] = updated;
        ApplyFilters(updated.Number);
    }
}

public sealed record TransportSummaryCardViewModel(string Label, string Value, string Caption, string AccentColor);

public sealed record TransportOrderRowViewModel(
    string Number,
    string OrderType,
    string Status,
    string StatusColor,
    string Customer,
    string RouteSummary,
    string LoadingWindow,
    string DeliveryWindow,
    string Vehicle,
    string Driver,
    string AmountLabel,
    bool IsArchived,
    bool IsUnassigned,
    bool RequiresAttention,
    string Carrier,
    string Trailer,
    string Incoterms,
    string Currency,
    string ExchangeRate,
    string ComplianceSummary,
    string Region,
    string HaulType,
    string Note,
    IReadOnlyList<TransportStopLineViewModel> Stops,
    IReadOnlyList<TransportDocumentLineViewModel> Documents,
    IReadOnlyList<TransportStatusHistoryLineViewModel> History)
{
    public string ModeSummary => string.Equals(OrderType, "Zagraniczne", StringComparison.OrdinalIgnoreCase)
        ? $"{Incoterms} · {Currency} · kurs {ExchangeRate}"
        : $"{Region} · {HaulType}";
}

public sealed record TransportStopLineViewModel(
    string Kind,
    string City,
    string Country,
    string PlannedTime);

public sealed record TransportDocumentLineViewModel(
    string Name,
    string State,
    string StateColor);

public sealed record TransportStatusHistoryLineViewModel(
    string Time,
    string State,
    string Actor);
