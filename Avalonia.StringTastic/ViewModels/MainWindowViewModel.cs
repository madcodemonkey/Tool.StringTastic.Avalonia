using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.StringTastic.Helpers;
using Avalonia.StringTastic.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Avalonia.StringTastic.ViewModels;

/// <summary>
/// Represents an open tab in the main window, wrapping a ToolItem with display metadata.
/// The actual UserControl content is created in code-behind and stored in the Content property.
/// </summary>
public class TabData : ObservableObject
{
    public ToolItem Tool { get; }
    public string Header { get; set; }

    /// <summary>
    /// The view control to display in the tab. Set by code-behind after creating the tab.
    /// </summary>
    public Control? Content
    {
        get => _content;
        set => SetProperty(ref _content, value);
    }
    private Control? _content;

    public TabData(ToolItem tool, string header)
    {
        Tool = tool;
        Header = header;
    }
}

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private Theme _currentTheme = Theme.Azure;

    [ObservableProperty]
    private TabData? _selectedTab;

    private readonly ObservableCollection<ToolItem> _allTools = new()
    {
        new ToolItem { DisplayName = "Compare", Type = ToolType.Compare, IconKey = "IconNewCompare" },
        new ToolItem { DisplayName = "Base64 Encode/Decode", Type = ToolType.Base64Encoder, IconKey = "IconEncodeDecode" },
        new ToolItem { DisplayName = "Color Picker", Type = ToolType.ColorPicker, IconKey = "IconEncodeDecode" },
        new ToolItem { DisplayName = "Generate GUIDs", Type = ToolType.GenerateGuid, IconKey = "IconGuid" },
        new ToolItem { DisplayName = "JWT Decode", Type = ToolType.JwtDecoder, IconKey = "IconEncodeDecode" },
        new ToolItem { DisplayName = "JSON Formatter", Type = ToolType.JsonFormatter, IconKey = "IconEncodeDecode" },
        new ToolItem { DisplayName = "Sorter", Type = ToolType.Sorter, IconKey = "IconSort" },
        new ToolItem { DisplayName = "Url Encode/Decode", Type = ToolType.UrlEncoder, IconKey = "IconEncodeDecode" },
    };

    public ObservableCollection<ToolItem> FilteredTools { get; } = new();

    /// <summary>
    /// Collection of open tabs. Each TabData holds metadata; the code-behind attaches the actual UserControl.
    /// </summary>
    public ObservableCollection<TabData> Tabs { get; } = new();

    // Per-tool-type counters for unique tab naming
    private int _compareCount;
    private int _base64EncoderCount;
    private int _colorPickerCount;
    private int _generateGuidCount;
    private int _jwtDecoderCount;
    private int _jsonFormatterCount;
    private int _sorterCount;
    private int _urlEncoderCount;

    public MainWindowViewModel()
    {
        foreach (var tool in _allTools)
            FilteredTools.Add(tool);

        CurrentTheme = ThemeManager.CurrentTheme;
    }

    partial void OnSearchTextChanged(string value)
    {
        FilteredTools.Clear();
        var filtered = string.IsNullOrWhiteSpace(value)
            ? _allTools
            : _allTools.Where(t => t.DisplayName.Contains(value, StringComparison.OrdinalIgnoreCase));

        foreach (var tool in filtered)
            FilteredTools.Add(tool);
    }

    [RelayCommand]
    private void ApplyTheme(string themeName)
    {
        if (Enum.TryParse<Theme>(themeName, out var theme))
        {
            ThemeManager.ApplyTheme(theme);
            CurrentTheme = theme;
        }
    }

    /// <summary>
    /// Adds a new tab for the given tool and returns the created TabData.
    /// The caller (code-behind) should then create the actual UserControl and
    /// set it as the TabItem's Content.
    /// </summary>
    public TabData AddTab(ToolItem tool)
    {
        var header = GetNextTabHeader(tool);
        var tab = new TabData(tool, header);
        Tabs.Add(tab);
        SelectedTab = tab;
        return tab;
    }

    public void CloseTab(TabData tab)
    {
        var index = Tabs.IndexOf(tab);
        Tabs.Remove(tab);

        // Select an adjacent tab
        if (Tabs.Count > 0)
        {
            SelectedTab = Tabs[Math.Min(index, Tabs.Count - 1)];
        }
        else
        {
            SelectedTab = null;
        }
    }

    public void CloseOtherTabs(TabData keepTab)
    {
        // Remove all tabs except keepTab
        for (int i = Tabs.Count - 1; i >= 0; i--)
        {
            if (Tabs[i] != keepTab)
                Tabs.RemoveAt(i);
        }
        SelectedTab = keepTab;
    }

    public void CloseTabsToRight(TabData anchorTab)
    {
        var index = Tabs.IndexOf(anchorTab);

        // Remove all tabs after the anchor
        for (int i = Tabs.Count - 1; i > index; i--)
        {
            Tabs.RemoveAt(i);
        }

        SelectedTab = anchorTab;
    }

    [RelayCommand]
    public void Close(TabData? tab)
    {
        if (tab is not null) CloseTab(tab);
    }

    [RelayCommand]
    public void CloseOthers(TabData? keepTab)
    {
        if (keepTab is not null) CloseOtherTabs(keepTab);
    }

    [RelayCommand]
    public void CloseToRight(TabData? anchorTab)
    {
        if (anchorTab is not null) CloseTabsToRight(anchorTab);
    }

    public void CloseAllTabs()
    {
        Tabs.Clear();
        SelectedTab = null;
    }

    private string GetNextTabHeader(ToolItem tool)
    {
        return tool.Type switch
        {
            ToolType.Compare => $"Compare {++_compareCount}",
            ToolType.Base64Encoder => $"Base64 Encoder {++_base64EncoderCount}",
            ToolType.ColorPicker => $"Color Picker {++_colorPickerCount}",
            ToolType.GenerateGuid => $"Generate GUID {++_generateGuidCount}",
            ToolType.JwtDecoder => $"JWT Decoder {++_jwtDecoderCount}",
            ToolType.JsonFormatter => $"JSON Formatter {++_jsonFormatterCount}",
            ToolType.Sorter => $"Sorter {++_sorterCount}",
            ToolType.UrlEncoder => $"URL Encoder {++_urlEncoderCount}",
            _ => tool.DisplayName
        };
    }
}