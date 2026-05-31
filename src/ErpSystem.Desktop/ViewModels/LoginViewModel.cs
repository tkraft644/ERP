using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErpSystem.Desktop.Services;

namespace ErpSystem.Desktop.ViewModels;

public sealed partial class LoginViewModel : ViewModelBase
{
    private readonly AuthApiClient authApiClient;
    private readonly SessionService sessionService;
    private readonly Action loginSucceeded;

    public LoginViewModel(AuthApiClient authApiClient, SessionService sessionService, Action loginSucceeded)
    {
        this.authApiClient = authApiClient;
        this.sessionService = sessionService;
        this.loginSucceeded = loginSucceeded;

        LoginCommand = new AsyncRelayCommand(LoginAsync, CanLogin);
        RefreshConnectionStatusCommand = new AsyncRelayCommand(RefreshConnectionStatusAsync);

        _ = RefreshConnectionStatusAsync();
    }

    [ObservableProperty]
    private string login = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private string apiStatusLabel = "Sprawdzanie połączenia";

    [ObservableProperty]
    private string apiStatusDescription = "Łączenie z lokalnym API...";

    [ObservableProperty]
    private bool isBusy;

    public string ApiEndpointLabel => authApiClient.BaseAddress;
    public IAsyncRelayCommand LoginCommand { get; }
    public IAsyncRelayCommand RefreshConnectionStatusCommand { get; }

    partial void OnLoginChanged(string value) => LoginCommand.NotifyCanExecuteChanged();
    partial void OnPasswordChanged(string value) => LoginCommand.NotifyCanExecuteChanged();

    private bool CanLogin()
    {
        return !IsBusy
               && !string.IsNullOrWhiteSpace(Login)
               && !string.IsNullOrWhiteSpace(Password);
    }

    private async Task RefreshConnectionStatusAsync()
    {
        var snapshot = await authApiClient.GetHealthAsync();
        ApiStatusLabel = snapshot.Label;
        ApiStatusDescription = snapshot.IsOnline
            ? snapshot.Description
            : $"{snapshot.Description} Upewnij się, że ErpSystem.Api działa pod {ApiEndpointLabel} i ma dostęp do SQL Servera.";
    }

    private async Task LoginAsync()
    {
        ErrorMessage = string.Empty;
        IsBusy = true;
        LoginCommand.NotifyCanExecuteChanged();

        try
        {
            var session = await authApiClient.LoginAsync(Login.Trim(), Password, CancellationToken.None);
            if (session is null)
            {
                ErrorMessage = "Niepoprawny login albo hasło.";
                return;
            }

            sessionService.SetSession(session);
            loginSucceeded();
        }
        catch (Exception exception)
        {
            ErrorMessage = $"Nie udało się zalogować: {TrimExceptionMessage(exception.Message)}";
            await RefreshConnectionStatusAsync();
        }
        finally
        {
            IsBusy = false;
            LoginCommand.NotifyCanExecuteChanged();
        }
    }

    private static string TrimExceptionMessage(string message)
    {
        return string.IsNullOrWhiteSpace(message) ? "Nieznany błąd połączenia z API." : message.Trim();
    }
}
