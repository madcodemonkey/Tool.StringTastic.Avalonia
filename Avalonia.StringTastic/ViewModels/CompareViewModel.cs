using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.StringTastic.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Avalonia.StringTastic.ViewModels;

public partial class CompareViewModel : ObservableObject
{
    [ObservableProperty]
    private string _leftText = string.Empty;

    [ObservableProperty]
    private string _rightText = string.Empty;

    /// <summary>
    /// Set by the view code-behind to allow the ViewModel to show dialogs.
    /// </summary>
    public Action<string, string>? ShowDialog { get; set; }

    /// <summary>
    /// Set by the view code-behind to allow the ViewModel to open file/folder dialogs.
    /// </summary>
    public Func<string, Task<string[]>>? PickFiles { get; set; }

    /// <summary>
    /// Set by the view code-behind to allow the ViewModel to open a folder picker dialog.
    /// </summary>
    public Func<string, Task<string?>>? PickFolder { get; set; }

    /// <summary>
    /// Set by the view code-behind to allow the ViewModel to open a single file picker for contents.
    /// </summary>
    public Func<string, Task<string?>>? PickFileForContents { get; set; }

    [RelayCommand]
    private void ClearLeft()
    {
        LeftText = string.Empty;
    }

    [RelayCommand]
    private void ClearRight()
    {
        RightText = string.Empty;
    }

    [RelayCommand]
    private void SortLeft()
    {
        if (string.IsNullOrEmpty(LeftText)) return;
        LeftText = LeftText.SortLines(sortAscending: true);
    }

    [RelayCommand]
    private void SortRight()
    {
        if (string.IsNullOrEmpty(RightText)) return;
        RightText = RightText.SortLines(sortAscending: true);
    }

    [RelayCommand]
    private void UniqueLeft()
    {
        if (string.IsNullOrEmpty(LeftText)) return;
        LeftText = LeftText.UniqueLines();
    }

    [RelayCommand]
    private void UniqueRight()
    {
        if (string.IsNullOrEmpty(RightText)) return;
        RightText = RightText.UniqueLines();
    }

    [RelayCommand]
    private void UniqueLeftToRight()
    {
        MoveUnique(LeftText, RightText, result => RightText = result);
    }

    [RelayCommand]
    private void UniqueRightToLeft()
    {
        MoveUnique(RightText, LeftText, result => LeftText = result);
    }

    [RelayCommand]
    private void TrimLeft()
    {
        if (string.IsNullOrEmpty(LeftText)) return;
        LeftText = LeftText.TrimLines();
    }

    [RelayCommand]
    private void TrimRight()
    {
        if (string.IsNullOrEmpty(RightText)) return;
        RightText = RightText.TrimLines();
    }

    [RelayCommand]
    private async Task LoadFileContentsLeft()
    {
        var content = await LoadFileContentsAsync();
        if (content != null)
            LeftText = content;
    }

    [RelayCommand]
    private async Task LoadFileContentsRight()
    {
        var content = await LoadFileContentsAsync();
        if (content != null)
            RightText = content;
    }

    [RelayCommand]
    private async Task LoadFileNamesLeft()
    {
        var names = await LoadFileNamesAsync();
        if (names != null)
            LeftText = string.Join("\n", names);
    }

    [RelayCommand]
    private async Task LoadFileNamesRight()
    {
        var names = await LoadFileNamesAsync();
        if (names != null)
            RightText = string.Join("\n", names);
    }

    [RelayCommand]
    private async Task LoadFolderNamesLeft()
    {
        var names = await LoadFolderNamesAsync();
        if (names != null)
            LeftText = string.Join("\n", names);
    }

    [RelayCommand]
    private async Task LoadFolderNamesRight()
    {
        var names = await LoadFolderNamesAsync();
        if (names != null)
            RightText = string.Join("\n", names);
    }

    [RelayCommand]
    private void ShowDifferences()
    {
        var leftStrings = LeftText.ToListOfString();
        var rightStrings = RightText.ToListOfString();

        var differences = new List<string>();

        differences.Add("In LEFT SIDE ONLY:");
        foreach (var item in leftStrings)
        {
            if (!rightStrings.Any(i => string.Equals(i, item, StringComparison.OrdinalIgnoreCase)))
                differences.Add(item);
        }

        differences.Add("--------------------------");
        differences.Add("In RIGHT SIDE ONLY:");
        foreach (var item in rightStrings)
        {
            if (!leftStrings.Any(i => string.Equals(i, item, StringComparison.OrdinalIgnoreCase)))
                differences.Add(item);
        }

        ShowDialog?.Invoke("Differences", string.Join("\n", differences));
    }

    [RelayCommand]
    private void ShowSimilarities()
    {
        var leftStrings = LeftText.ToListOfString();
        var rightStrings = RightText.ToListOfString();

        var similarities = new List<string>();

        foreach (var item in leftStrings)
        {
            if (rightStrings.Any(i => string.Equals(i, item, StringComparison.OrdinalIgnoreCase)))
                similarities.Add(item);
        }

        ShowDialog?.Invoke("Similarities", string.Join("\n", similarities));
    }

    private void MoveUnique(string source, string destination, Action<string> setResult)
    {
        var listOfStrings = source.ToListOfString();
        var distinct = listOfStrings.Distinct();
        setResult(string.Join("\n", distinct));
    }

    private async Task<string?> LoadFileContentsAsync()
    {
        if (PickFileForContents == null) return null;
        return await PickFileForContents("Open File");
    }

    private async Task<string[]?> LoadFileNamesAsync()
    {
        if (PickFiles == null) return null;
        return await PickFiles("Select files");
    }

    private async Task<string[]?> LoadFolderNamesAsync()
    {
        if (PickFolder == null) return null;
        var folder = await PickFolder("Select a directory");
        if (folder == null) return null;
        try
        {
            return Directory.GetDirectories(folder)
                .Select(d => new DirectoryInfo(d).Name)
                .ToArray();
        }
        catch
        {
            return null;
        }
    }
}