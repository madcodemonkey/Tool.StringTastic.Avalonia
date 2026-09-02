using System;
using System.Text;
using Avalonia.StringTastic.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Avalonia.StringTastic.ViewModels;

public partial class Base64EncoderViewModel : ObservableObject
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
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        OutputText = Convert.ToBase64String(plainTextBytes);
    }

    [RelayCommand]
    private void Decode()
    {
        try
        {
            var base64EncodedData = InputText.ToOneString(true);
            var result = Base64Decode(base64EncodedData);
            OutputText = result;
        }
        catch (Exception ex)
        {
            OutputText = "----------------\n" + ex.Message;
        }
    }

    [RelayCommand]
    private void TrimInput()
    {
        if (string.IsNullOrEmpty(InputText)) return;
        InputText = InputText.TrimLines();
    }

    private static string Base64Decode(string base64EncodedData)
    {
        int mod4 = base64EncodedData.Length % 4;
        if (mod4 > 0)
        {
            base64EncodedData += new string('=', 4 - mod4);
        }

        var plainTextBytes = Convert.FromBase64String(base64EncodedData);
        return Encoding.UTF8.GetString(plainTextBytes);
    }
}