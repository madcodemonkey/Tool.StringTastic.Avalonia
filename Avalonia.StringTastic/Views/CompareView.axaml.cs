using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Platform.Storage;
using Avalonia.StringTastic.Dialogs;
using Avalonia.StringTastic.ViewModels;
using Avalonia.VisualTree;

namespace Avalonia.StringTastic.Views;

public partial class CompareView : UserControl
{
    private CompareViewModel _vm = null!;

    public CompareView()
    {
        InitializeComponent();
        _vm = new CompareViewModel
        {
            ShowDialog = ShowTextDialog,
            PickFiles = PickFilesAsync,
            PickFolder = PickFolderAsync,
            PickFileForContents = PickFileForContentsAsync,
        };
        DataContext = _vm;

        LeftTextBox.ContextMenu = CreateTextBoxContextMenu(isLeft: true);
        RightTextBox.ContextMenu = CreateTextBoxContextMenu(isLeft: false);
    }

    private ContextMenu CreateTextBoxContextMenu(bool isLeft)
    {
        var menu = new ContextMenu();

        var selectAll = new MenuItem { Header = "Select All" };
        selectAll.Click += (s, e) =>
        {
            var tb = isLeft ? LeftTextBox : RightTextBox;
            tb.SelectAll();
        };

        var copy = new MenuItem { Header = "Copy" };
        copy.Click += async (s, e) =>
        {
            var tb = isLeft ? LeftTextBox : RightTextBox;
            var text = tb.SelectedText?.Length > 0 ? tb.SelectedText : tb.Text;
            if (!string.IsNullOrEmpty(text))
            {
                var topLevel = TopLevel.GetTopLevel(this);
                if (topLevel?.Clipboard != null)
                    await topLevel.Clipboard.SetTextAsync(text);
            }
        };

        var cut = new MenuItem { Header = "Cut" };
        cut.Click += async (s, e) =>
        {
            var tb = isLeft ? LeftTextBox : RightTextBox;
            if (tb.SelectedText?.Length > 0)
            {
                var topLevel = TopLevel.GetTopLevel(this);
                if (topLevel?.Clipboard != null)
                    await topLevel.Clipboard.SetTextAsync(tb.SelectedText);
                var start = Math.Min(tb.SelectionStart, tb.SelectionEnd);
                var len = Math.Abs(tb.SelectionEnd - tb.SelectionStart);
                tb.Text = tb.Text.Remove(start, len);
            }
        };

        var paste = new MenuItem { Header = "Paste" };
        paste.Click += async (s, e) =>
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel?.Clipboard == null) return;
            var text = await topLevel.Clipboard.TryGetTextAsync();
            if (text != null)
            {
                var tb = isLeft ? LeftTextBox : RightTextBox;
                if (tb.SelectedText?.Length > 0)
                {
                    var start = Math.Min(tb.SelectionStart, tb.SelectionEnd);
                    var len = Math.Abs(tb.SelectionEnd - tb.SelectionStart);
                    tb.Text = tb.Text.Remove(start, len).Insert(start, text);
                }
                else
                {
                    tb.Text += text;
                }
            }
        };

        var separator = new Separator();

        var loadMenu = new MenuItem { Header = "Load" };

        var loadContents = new MenuItem { Header = "Contents of a file" };
        loadContents.Click += async (s, e) =>
        {
            if (isLeft) await _vm.LoadFileContentsLeftCommand.ExecuteAsync(null);
            else await _vm.LoadFileNamesRightCommand.ExecuteAsync(null);
        };

        var loadFileNames = new MenuItem { Header = "Pick file names within a folder" };
        loadFileNames.Click += async (s, e) =>
        {
            if (isLeft) await _vm.LoadFileNamesLeftCommand.ExecuteAsync(null);
            else await _vm.LoadFileNamesRightCommand.ExecuteAsync(null);
        };

        var loadFolderNames = new MenuItem { Header = "Get child folder names (select parent)" };
        loadFolderNames.Click += async (s, e) =>
        {
            if (isLeft) await _vm.LoadFolderNamesLeftCommand.ExecuteAsync(null);
            else await _vm.LoadFolderNamesRightCommand.ExecuteAsync(null);
        };

        loadMenu.ItemsSource = new[] { loadContents, loadFileNames, loadFolderNames };
        menu.ItemsSource = new object[] { selectAll, copy, cut, paste, separator, loadMenu };

        return menu;
    }

    private void ShowTextDialog(string title, string content)
    {
        var items = content.Split('\n').ToList();
        var dialog = new TextDialogBox(items, title);
        var ownerWindow = this.FindAncestorOfType<Window>();
        if (ownerWindow is not null)
            dialog.ShowDialog(ownerWindow);
        else
            dialog.Show();
    }

    private async Task<string[]> PickFilesAsync(string title)
    {
        var window = this.FindAncestorOfType<Window>();
        if (window == null) return Array.Empty<string>();

        var storage = window.StorageProvider;
        var files = await storage.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = true,
        });

        return files.Select(f => f.TryGetLocalPath() ?? f.Name).ToArray();
    }

    private async Task<string?> PickFolderAsync(string title)
    {
        // Try Avalonia's native folder picker first
        var window = this.FindAncestorOfType<Window>();
        if (window != null)
        {
            var storage = window.StorageProvider;
            try
            {
                var folders = await storage.OpenFolderPickerAsync(new FolderPickerOpenOptions
                {
                    Title = title,
                    AllowMultiple = false,
                });
                if (folders.Count > 0)
                {
                    var path = folders[0].TryGetLocalPath();
                    if (path != null && Directory.Exists(path))
                        return path;
                }
            }
            catch
            {
                // Native folder picker failed — fall through to zenity
            }
        }

        // Fall back to zenity (works on GNOME/Linux)
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "zenity",
                Arguments = "--file-selection --directory --title=\"" + title + "\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };
            var process = Process.Start(psi);
            if (process == null) return null;
            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();
            if (process.ExitCode == 0 && !string.IsNullOrWhiteSpace(output))
            {
                var dir = output.Trim();
                if (Directory.Exists(dir))
                    return dir;
            }
        }
        catch
        {
            // zenity not available
        }

        return null;
    }

    private async Task<string?> PickFileForContentsAsync(string title)
    {
        var window = this.FindAncestorOfType<Window>();
        if (window == null) return null;

        var storage = window.StorageProvider;
        var files = await storage.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = false,
        });

        if (files.Count == 0) return null;

        var filePath = files[0].TryGetLocalPath();
        if (filePath == null) return null;

        try
        {
            return await File.ReadAllTextAsync(filePath);
        }
        catch
        {
            return null;
        }
    }
}