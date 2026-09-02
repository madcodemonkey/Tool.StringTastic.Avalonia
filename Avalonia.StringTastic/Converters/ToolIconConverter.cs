using System;
using System.Collections.Generic;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.StringTastic.Models;
using CultureInfo = System.Globalization.CultureInfo;

namespace Avalonia.StringTastic.Converters;

/// <summary>
/// Converts a ToolType to a unicode symbol string for display in the sidebar.
/// </summary>
public class ToolIconConverter : IValueConverter
{
    private static readonly Dictionary<ToolType, string> Icons = new()
    {
        [ToolType.Sorter] = "📊",       // Bar chart
        [ToolType.Compare] = "⇄",       // Arrows
        [ToolType.Base64Encoder] = "{}",  // Braces
        [ToolType.ColorPicker] = "🎨",   // Palette
        [ToolType.GenerateGuid] = "🆔",  // ID
        [ToolType.JwtDecoder] = "🔑",   // Key
        [ToolType.UrlEncoder] = "🔗",   // Link
    };

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ToolType type && Icons.TryGetValue(type, out var icon))
            return icon;
        return "🔧";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}