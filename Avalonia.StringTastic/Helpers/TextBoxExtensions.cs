using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Avalonia.StringTastic.Helpers;

public static class TextBoxExtensions
{
    /// <summary>Gets all lines from a TextBox as a List of strings.</summary>
    public static List<string> ToListOfString(this string text)
    {
        if (string.IsNullOrEmpty(text))
            return new List<string>();

        return text.Split('\n').ToList();
    }

    /// <summary>Gets all text as a single string, optionally ignoring blank lines.</summary>
    public static string ToOneString(this string text, bool ignoreBlankLines)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        var sb = new StringBuilder();
        var lines = text.Split('\n');

        foreach (var line in lines)
        {
            if (ignoreBlankLines && string.IsNullOrWhiteSpace(line))
                continue;
            sb.Append(line);
        }

        return sb.ToString();
    }

    /// <summary>Sorts lines in ascending or descending order.</summary>
    public static string SortLines(this string text, bool sortAscending)
    {
        var lines = text.Split('\n').ToList();

        // Remove trailing blank lines
        while (lines.Count > 0 && string.IsNullOrWhiteSpace(lines[lines.Count - 1]))
            lines.RemoveAt(lines.Count - 1);

        var sorted = sortAscending
            ? lines.OrderBy(item => item).ToList()
            : lines.OrderByDescending(item => item).ToList();

        return string.Join("\n", sorted);
    }

    /// <summary>Returns distinct lines, preserving order.</summary>
    public static string UniqueLines(this string text)
    {
        var lines = text.Split('\n');
        var distinct = lines.Distinct();
        return string.Join("\n", distinct);
    }

    /// <summary>Trims whitespace from each line, removing blank lines.</summary>
    public static string TrimLines(this string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        var lines = text.Split('\n');
        var sb = new StringBuilder();

        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (!string.IsNullOrEmpty(trimmed))
                sb.AppendLine(trimmed);
        }

        return sb.ToString().TrimEnd('\r', '\n');
    }
}