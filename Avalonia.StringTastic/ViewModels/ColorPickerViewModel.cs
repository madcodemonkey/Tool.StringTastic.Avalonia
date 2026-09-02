using System;
using System.Threading.Tasks;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Avalonia.StringTastic.ViewModels;

public partial class ColorPickerViewModel : ObservableObject
{
    [ObservableProperty]
    private string _hexColorText = "#1ea54c";

    [ObservableProperty]
    private Color _selectedColor = Color.FromRgb(0x1e, 0xa5, 0x4c);

    /// <summary>
    /// Brush version of SelectedColor for binding to Background properties in XAML.
    /// </summary>
    public SolidColorBrush SelectedColorBrush => new(SelectedColor);

    /// <summary>
    /// Set by the view code-behind to allow clipboard copy.
    /// </summary>
    public Func<string, Task>? CopyToClipboard { get; set; }

    /// <summary>
    /// Set by the view code-behind to open a color picker dialog.
    /// </summary>
    public Func<Color, Task<Color?>>? OpenColorPicker { get; set; }

    partial void OnSelectedColorChanged(Color value)
    {
        HexColorText = FormatColor(value);
        OnPropertyChanged(nameof(SelectedColorBrush));
    }

    [RelayCommand]
    private void Apply()
    {
        try
        {
            var hexColor = HexColorText.Trim();
            if (string.IsNullOrWhiteSpace(hexColor)) return;

            var color = ParseColor(hexColor);
            SelectedColor = color;
            HexColorText = FormatColor(color);
        }
        catch
        {
            // Invalid hex color format - ignore
        }
    }

    [RelayCommand]
    private void Clear()
    {
        HexColorText = string.Empty;
        SelectedColor = Colors.Black;
    }

    [RelayCommand]
    private async Task Copy()
    {
        if (!string.IsNullOrWhiteSpace(HexColorText) && CopyToClipboard is not null)
        {
            await CopyToClipboard(HexColorText);
        }
    }

    [RelayCommand]
    private async Task PickColor()
    {
        if (OpenColorPicker is not null)
        {
            var result = await OpenColorPicker(SelectedColor);
            if (result is not null)
            {
                SelectedColor = result.Value;
                HexColorText = FormatColor(result.Value);
            }
        }
    }

    private static Color ParseColor(string hexColor)
    {
        var hex = hexColor.TrimStart('#');

        return hex.Length switch
        {
            6 => Color.FromRgb(
                Convert.ToByte(hex.Substring(0, 2), 16),
                Convert.ToByte(hex.Substring(2, 2), 16),
                Convert.ToByte(hex.Substring(4, 2), 16)),
            8 => Color.FromArgb(
                Convert.ToByte(hex.Substring(0, 2), 16),
                Convert.ToByte(hex.Substring(2, 2), 16),
                Convert.ToByte(hex.Substring(4, 2), 16),
                Convert.ToByte(hex.Substring(6, 2), 16)),
            _ => throw new FormatException("Invalid hex color format")
        };
    }

    private static string FormatColor(Color color)
    {
        return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
    }
}