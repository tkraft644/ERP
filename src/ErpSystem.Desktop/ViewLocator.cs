using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using ErpSystem.Desktop.ViewModels;
using ErpSystem.Desktop.Views;

namespace ErpSystem.Desktop;

public class ViewLocator : IDataTemplate
{

    public Control? Build(object? param)
    {
        if (param is null)
            return null;
        
        var name = param.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
        var type = Type.GetType(name);

        if (type != null)
        {
            return (Control)Activator.CreateInstance(type)!;
        }

        if (param is ModuleWorkspaceViewModelBase)
        {
            return new ModuleWorkspaceView();
        }
        
        return new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
