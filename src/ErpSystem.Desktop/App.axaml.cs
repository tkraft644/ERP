using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using ErpSystem.Desktop.Services;
using ErpSystem.Desktop.Views;
using ErpSystem.Desktop.ViewModels;

namespace ErpSystem.Desktop;

public partial class App : Avalonia.Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            var sessionService = new SessionService();
            var dialogService = new DialogService();
            var navigationService = new NavigationService();
            var authApiClient = new AuthApiClient();
            var apiClient = new ApiClient(sessionService);
            var warehouseWorkspaceStore = new WarehouseWorkspaceStore(apiClient);
            var menuBuilder = new ShellMenuBuilder();
            var themeService = new ThemeService();
            var appViewModel = new AppViewModel(
                onLoginSucceeded => new LoginViewModel(authApiClient, sessionService, onLoginSucceeded),
                onSignOut => new ShellViewModel(
                    navigationService,
                    dialogService,
                    apiClient,
                    menuBuilder,
                    themeService,
                    sessionService,
                    warehouseWorkspaceStore,
                    onSignOut));
            desktop.MainWindow = new MainWindow
            {
                DataContext = appViewModel,
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}
