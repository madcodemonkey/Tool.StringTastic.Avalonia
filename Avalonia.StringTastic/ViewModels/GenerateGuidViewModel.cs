using System;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Avalonia.StringTastic.ViewModels;

public partial class GenerateGuidViewModel : ObservableObject
{
    [ObservableProperty]
    private string _outputText = string.Empty;

    [RelayCommand]
    private void Clear()
    {
        OutputText = string.Empty;
    }

    [RelayCommand]
    private void Generate()
    {
        var sb = new StringBuilder();

        sb.AppendLine("10 new GUIDs style D (lowercase)");
        for (int i = 0; i < 10; i++)
        {
            sb.AppendLine(Guid.NewGuid().ToString("D").ToLower());
        }

        sb.AppendLine();
        sb.AppendLine("10 new GUIDs style D (uppercase)");
        for (int i = 0; i < 10; i++)
        {
            sb.AppendLine(Guid.NewGuid().ToString("D").ToUpper());
        }

        sb.AppendLine();
        sb.AppendLine("10 new GUIDs style N (lowercase)");
        for (int i = 0; i < 10; i++)
        {
            sb.AppendLine(Guid.NewGuid().ToString("N").ToLower());
        }

        sb.AppendLine();
        sb.AppendLine("10 new GUIDs style N (uppercase)");
        for (int i = 0; i < 10; i++)
        {
            sb.AppendLine(Guid.NewGuid().ToString("N").ToUpper());
        }

        OutputText = sb.ToString();
    }
}