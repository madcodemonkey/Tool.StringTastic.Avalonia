using System.Net;
using Avalonia.StringTastic.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Avalonia.StringTastic.ViewModels;

public partial class UrlEncoderViewModel : ObservableObject
{
    [ObservableProperty]
    private string _inputText = string.Empty;

    [ObservableProperty]
    private string _outputText = string.Empty;

    [RelayCommand]
    private void ClearInput()
    {
        InputText = string.Empty;
    }

    [RelayCommand]
    private void ClearOutput()
    {
        OutputText = string.Empty;
    }

    [RelayCommand]
    private void Encode()
    {
        var plainText = InputText.ToOneString(true);
        OutputText = WebUtility.UrlEncode(plainText);
    }

    [RelayCommand]
    private void Decode()
    {
        var encodedData = InputText.ToOneString(true);
        OutputText = WebUtility.UrlDecode(encodedData);
    }

    [RelayCommand]
    private void TrimInput()
    {
        if (string.IsNullOrEmpty(InputText)) return;
        InputText = InputText.TrimLines();
    }
}