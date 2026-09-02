using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.StringTastic.Helpers;
using Avalonia.StringTastic.ViewModels;
using Avalonia.StringTastic.Views;

namespace Avalonia.StringTastic;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        // Load and apply saved theme preference
        var savedTheme = ThemeManager.LoadThemePreference();
        ThemeManager.ApplyTheme(savedTheme);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}