using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace Avalonia.StringTastic.Dialogs;

public partial class TextDialogBox : Window
{
    public TextDialogBox()
    {
        InitializeComponent();
    }

    public TextDialogBox(List<string> items, string title) : this()
    {
        Title = title;
        ItemCountLabel.Content = $"{items.Count} items listed";
        ContentTextBox.Text = string.Join(Environment.NewLine, items);
    }

    private void CloseButton_Click(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }

    private async void SaveButton_Click(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
    {
        await SaveContentAsync();
    }

    private async Task SaveContentAsync()
    {
        var topLevel = TopLevel.GetTopLevel(this)
            ?? throw new InvalidOperationException("Cannot find TopLevel for the control.");

        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save items",
            SuggestedFileName = "items.txt",
            FileTypeChoices = new[]
            {
                new FilePickerFileType("Text files")
                {
                    Patterns = new[] { "*.txt" }
                },
                new FilePickerFileType("All files")
                {
                    Patterns = new[] { "*.*" }
                }
            }
        });

        if (file is null)
            return;

        await using var stream = await file.OpenWriteAsync();
        using var writer = new StreamWriter(stream);
        await writer.WriteAsync(ContentTextBox.Text);
    }
}