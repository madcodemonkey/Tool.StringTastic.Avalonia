using System;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Newtonsoft.Json;

namespace Avalonia.StringTastic.Helpers;

public enum Theme { Light, Dark, Azure }

public static class ThemeManager
{
    private const string ThemeResourcePrefix = "avares://Avalonia.StringTastic/Themes/";
    private static Theme _currentTheme = Theme.Azure;
    private static ResourceDictionary? _currentThemeDict;
    
    public static Theme CurrentTheme => _currentTheme;

    public static Theme LoadThemePreference()
    {
        try
        {
            var path = GetSettingsPath();
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                var settings = JsonConvert.DeserializeObject<Settings>(json);
                if (Enum.TryParse<Theme>(settings?.SelectedTheme, out var theme))
                    return theme;
            }
        }
        catch { }

        return Theme.Azure;
    }

    public static void SaveThemePreference(Theme theme)
    {
        try
        {
            var path = GetSettingsPath();
            var dir = Path.GetDirectoryName(path)!;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var settings = new Settings { SelectedTheme = theme.ToString() };
            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(path, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to save theme preference: {ex.Message}");
        }
    }

    public static void ApplyTheme(Theme theme)
    {
        // Remove the previous theme dictionary
        if (_currentThemeDict is not null)
        {
            Application.Current!.Resources.MergedDictionaries.Remove(_currentThemeDict);
        }

        var themeFile = GetThemeFileName(theme);
        var uri = new Uri($"{ThemeResourcePrefix}{themeFile}");
        _currentThemeDict = (ResourceDictionary)AvaloniaXamlLoader.Load(uri);

        Application.Current!.Resources.MergedDictionaries.Add(_currentThemeDict);
        _currentTheme = theme;
        SaveThemePreference(theme);
    }

    private static string GetThemeFileName(Theme theme) => theme switch
    {
        Theme.Light => "LightTheme.axaml",
        Theme.Dark => "DarkTheme.axaml",
        Theme.Azure => "AzureTheme.axaml",
        _ => throw new ArgumentOutOfRangeException(nameof(theme))
    };

    public static string GetThemeDisplayName(Theme theme) => theme switch
    {
        Theme.Light => "Light",
        Theme.Dark => "Dark",
        Theme.Azure => "Azure",
        _ => theme.ToString()
    };

    private static string GetSettingsPath()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(appData, "StringTastic", "settings.json");
    }

    private class Settings
    {
        public string? SelectedTheme { get; set; }
    }
}