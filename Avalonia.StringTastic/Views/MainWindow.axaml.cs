using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.StringTastic.Helpers;
using Avalonia.StringTastic.Models;
using Avalonia.StringTastic.ViewModels;
using ThemeEnum = Avalonia.StringTastic.Helpers.Theme;

namespace Avalonia.StringTastic.Views;

public partial class MainWindow : Window
{
    private MainWindowViewModel? ViewModel => DataContext as MainWindowViewModel;

    public MainWindow()
    {
        InitializeComponent();

        Loaded += (_, _) =>
        {
            UpdateThemeMenuCheckmarks();

            // Create initial tabs like the WPF version
            AddTabForTool(ToolType.GenerateGuid);
            AddTabForTool(ToolType.JwtDecoder);
        };
    }

    #region Tool selection

    private void ToolsListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (ToolsListBox.SelectedItem is ToolItem tool)
        {
            AddTabForTool(tool.Type);
            ToolsListBox.SelectedItem = null;
        }
    }

    private void ToolsListBox_DoubleTapped(object? sender, TappedEventArgs e)
    {
        // DoubleTapped provides an additional way to open a tool
        if (ToolsListBox.SelectedItem is ToolItem tool)
        {
            AddTabForTool(tool.Type);
            ToolsListBox.SelectedItem = null;
        }
    }

    private static readonly ToolItem[] AllTools =
    [
        new() { DisplayName = "Compare", Type = ToolType.Compare, IconKey = "IconNewCompare" },
        new() { DisplayName = "Base64 Encode/Decode", Type = ToolType.Base64Encoder, IconKey = "IconEncodeDecode" },
        new() { DisplayName = "Color Picker", Type = ToolType.ColorPicker, IconKey = "IconEncodeDecode" },
        new() { DisplayName = "Generate GUIDs", Type = ToolType.GenerateGuid, IconKey = "IconGuid" },
        new() { DisplayName = "JWT Decode", Type = ToolType.JwtDecoder, IconKey = "IconEncodeDecode" },
        new() { DisplayName = "JSON Formatter", Type = ToolType.JsonFormatter, IconKey = "IconEncodeDecode" },
        new() { DisplayName = "Sorter", Type = ToolType.Sorter, IconKey = "IconSort" },
        new() { DisplayName = "Url Encode/Decode", Type = ToolType.UrlEncoder, IconKey = "IconEncodeDecode" },
    ];

    private void AddTabForTool(ToolType toolType)
    {
        if (ViewModel is null) return;

        var tool = AllTools.FirstOrDefault(t => t.Type == toolType);
        if (tool is null) return;

        var tabData = ViewModel.AddTab(tool);
        tabData.Content = CreateViewForTool(toolType);
    }

    private Control CreateViewForTool(ToolType toolType)
    {
        return toolType switch
        {
            ToolType.Compare => new CompareView(),
            ToolType.Base64Encoder => new Base64EncoderView(),
            ToolType.ColorPicker => new ColorPickerView(),
            ToolType.GenerateGuid => new GenerateGuidView(),
            ToolType.JwtDecoder => new JwtDecoderView(),
            ToolType.JsonFormatter => new JsonFormatterView(),
            ToolType.Sorter => new SorterView(),
            ToolType.UrlEncoder => new UrlEncoderView(),
            _ => CreatePlaceholder(toolType.ToString() ?? "Unknown")
        };
    }

    private static TextBlock CreatePlaceholder(string toolName)
    {
        return new TextBlock
        {
            Text = $"{toolName} - View not yet implemented",
            HorizontalAlignment = Layout.HorizontalAlignment.Center,
            VerticalAlignment = Layout.VerticalAlignment.Center,
            FontSize = 16,
            Foreground = new Media.SolidColorBrush(Media.Color.FromRgb(0x99, 0x99, 0x99))
        };
    }

    #endregion

    #region Tab close

    private void CloseTabButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is TabData tabData && ViewModel is not null)
        {
            ViewModel.CloseTab(tabData);
        }
    }

    #endregion

    #region Menu handlers

    private void Exit_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ThemeLight_Click(object? sender, RoutedEventArgs e)
    {
        ThemeManager.ApplyTheme(ThemeEnum.Light);
        UpdateThemeMenuCheckmarks();
        if (ViewModel is not null)
            ViewModel.CurrentTheme = ThemeEnum.Light;
    }

    private void ThemeDark_Click(object? sender, RoutedEventArgs e)
    {
        ThemeManager.ApplyTheme(ThemeEnum.Dark);
        UpdateThemeMenuCheckmarks();
        if (ViewModel is not null)
            ViewModel.CurrentTheme = ThemeEnum.Dark;
    }

    private void ThemeAzure_Click(object? sender, RoutedEventArgs e)
    {
        ThemeManager.ApplyTheme(ThemeEnum.Azure);
        UpdateThemeMenuCheckmarks();
        if (ViewModel is not null)
            ViewModel.CurrentTheme = ThemeEnum.Azure;
    }

    private void UpdateThemeMenuCheckmarks()
    {
        var current = ThemeManager.CurrentTheme;
        MenuItemLightTheme.IsChecked = current == ThemeEnum.Light;
        MenuItemDarkTheme.IsChecked = current == ThemeEnum.Dark;
        MenuItemAzureTheme.IsChecked = current == ThemeEnum.Azure;
    }

    #endregion
}