using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ErpSystem.Desktop.ViewModels;

public sealed partial class ShellMenuItemViewModel : ViewModelBase
{
    public ShellMenuItemViewModel(string title, string subtitle, string route, Action<ShellMenuItemViewModel> openAction)
    {
        Title = title;
        Subtitle = subtitle;
        Route = route;
        OpenCommand = new RelayCommand(() => openAction(this));
    }

    public string Title { get; }
    public string Subtitle { get; }
    public string Route { get; }
    public IRelayCommand OpenCommand { get; }

    [ObservableProperty]
    private bool isSelected;

    public string Background => IsSelected ? "#DCEBFF" : "Transparent";
    public string BorderBrush => IsSelected ? "#0F6CBD" : "#D7E2F2";
    public string Foreground => IsSelected ? "#0C3B71" : "#223246";

    partial void OnIsSelectedChanged(bool value)
    {
        OnPropertyChanged(nameof(Background));
        OnPropertyChanged(nameof(BorderBrush));
        OnPropertyChanged(nameof(Foreground));
    }
}
