using CommunityToolkit.Mvvm.ComponentModel;

namespace ErpSystem.Desktop.ViewModels;

public sealed partial class AppViewModel : ViewModelBase
{
    private readonly Func<Action, LoginViewModel> loginViewModelFactory;
    private readonly Func<Action, ShellViewModel> shellViewModelFactory;

    public AppViewModel(
        Func<Action, LoginViewModel> loginViewModelFactory,
        Func<Action, ShellViewModel> shellViewModelFactory)
    {
        this.loginViewModelFactory = loginViewModelFactory;
        this.shellViewModelFactory = shellViewModelFactory;

        ShowLogin();
    }

    [ObservableProperty]
    private ViewModelBase currentViewModel = null!;

    private void ShowLogin()
    {
        CurrentViewModel = loginViewModelFactory(ShowShell);
    }

    private void ShowShell()
    {
        CurrentViewModel = shellViewModelFactory(ShowLogin);
    }
}
