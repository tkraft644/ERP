using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErpSystem.Desktop.Services;

namespace ErpSystem.Desktop.ViewModels;

public sealed partial class DashboardWorkspaceViewModel : WorkspaceViewModelBase
{
    private readonly ApiClient apiClient;
    private readonly Action<string>? openWorkspace;

    public DashboardWorkspaceViewModel(ApiClient apiClient, Action<string>? openWorkspace = null)
        : base(
            "/dashboard",
            "Start",
            "Dashboard",
            "Pierwszy ekran po zalogowaniu. Pokazuje tylko realne liczniki z bazy SQL.",
            false)
    {
        this.apiClient = apiClient;
        this.openWorkspace = openWorkspace;

        RefreshCommand = new AsyncRelayCommand(LoadAsync);
        OpenWorkspaceCommand = new RelayCommand<string>(route =>
        {
            if (!string.IsNullOrWhiteSpace(route))
            {
                this.openWorkspace?.Invoke(route);
            }
        });

        _ = LoadAsync();
    }

    [ObservableProperty]
    private int productCount;

    [ObservableProperty]
    private int draftWarehouseDocumentCount;

    [ObservableProperty]
    private int transportOrderCount;

    [ObservableProperty]
    private int activeEmployeeCount;

    [ObservableProperty]
    private string statusMessage = "Ładowanie danych dashboardu...";

    [ObservableProperty]
    private bool isBusy;

    public bool HasAnyData => ProductCount > 0 || DraftWarehouseDocumentCount > 0 || TransportOrderCount > 0 || ActiveEmployeeCount > 0;
    public string EmptyStateMessage => "Brak danych. Dodaj pierwszy produkt lub dokument, aby rozpocząć pracę.";
    public IAsyncRelayCommand RefreshCommand { get; }
    public IRelayCommand<string> OpenWorkspaceCommand { get; }

    private async Task LoadAsync()
    {
        try
        {
            IsBusy = true;
            var summary = await apiClient.GetDashboardSummaryAsync();
            ProductCount = summary.ProductCount;
            DraftWarehouseDocumentCount = summary.DraftWarehouseDocumentCount;
            TransportOrderCount = summary.TransportOrderCount;
            ActiveEmployeeCount = summary.ActiveEmployeeCount;
            StatusMessage = HasAnyData
                ? "Liczniki pochodzą z aktualnej bazy SQL i API."
                : EmptyStateMessage;
        }
        catch (Exception exception)
        {
            ProductCount = 0;
            DraftWarehouseDocumentCount = 0;
            TransportOrderCount = 0;
            ActiveEmployeeCount = 0;
            StatusMessage = $"Nie udało się pobrać dashboardu: {TrimExceptionMessage(exception.Message)}";
        }
        finally
        {
            IsBusy = false;
            OnPropertyChanged(nameof(HasAnyData));
            OnPropertyChanged(nameof(EmptyStateMessage));
        }
    }

    private static string TrimExceptionMessage(string message)
    {
        return string.IsNullOrWhiteSpace(message) ? "Nieznany błąd." : message.Trim();
    }
}
