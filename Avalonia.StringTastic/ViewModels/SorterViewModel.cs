using System.Linq;
using Avalonia.StringTastic.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Avalonia.StringTastic.ViewModels;

public partial class SorterViewModel : ObservableObject
{
    [ObservableProperty]
    private string _inputText = string.Empty;

    [RelayCommand]
    private void Clear()
    {
        InputText = string.Empty;
    }

    [RelayCommand]
    private void SortAscending()
    {
        if (string.IsNullOrEmpty(InputText)) return;
        InputText = InputText.SortLines(sortAscending: true);
    }

    [RelayCommand]
    private void SortDescending()
    {
        if (string.IsNullOrEmpty(InputText)) return;
        InputText = InputText.SortLines(sortAscending: false);
    }

    [RelayCommand]
    private void Unique()
    {
        if (string.IsNullOrEmpty(InputText)) return;
        InputText = InputText.UniqueLines();
    }

    [RelayCommand]
    private void Trim()
    {
        if (string.IsNullOrEmpty(InputText)) return;
        InputText = InputText.TrimLines();
    }
}