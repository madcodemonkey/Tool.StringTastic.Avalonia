using System;
using System.Text;
using Avalonia.StringTastic.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json.Linq;

namespace Avalonia.StringTastic.ViewModels;

public partial class JwtDecoderViewModel : ObservableObject
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
    private void Decode()
    {
        var sb = new StringBuilder();

        try
        {
            var encodedData = InputText.ToOneString(true).Trim();
            var encodedParts = encodedData.Split('.');

            if (encodedParts.Length != 3)
            {
                OutputText = "Invalid JWT token. It should have three distinct parts!";
                return;
            }

            var part1 = Base64Decode(encodedParts[0]);
            var part2 = Base64Decode(encodedParts[1]);

            sb.AppendLine("Header:");
            var part1Object = JObject.Parse(part1);
            sb.AppendLine(part1Object.ToString());

            sb.AppendLine();
            sb.AppendLine("Payload:");
            var part2Object = JObject.Parse(part2);
            sb.AppendLine(part2Object.ToString());

            sb.AppendLine();
            sb.AppendLine("Signature:");
            sb.AppendLine("[Encoded Signature]");

            DecodeDate(sb, "Payload Issued date (iat)", "iat", part2Object);
            DecodeDate(sb, "Payload Expiration date (exp)", "exp", part2Object);

            OutputText = sb.ToString();
        }
        catch (Exception ex)
        {
            sb.Clear();
            sb.AppendLine("----------------");
            sb.AppendLine(ex.Message);
            OutputText = sb.ToString();
        }
    }

    [RelayCommand]
    private void TrimInput()
    {
        if (string.IsNullOrEmpty(InputText)) return;
        InputText = InputText.TrimLines();
    }

    private static void DecodeDate(StringBuilder sb, string title, string propertyName, JObject payload)
    {
        sb.AppendLine();
        var token = payload[propertyName];

        if (token == null || string.IsNullOrWhiteSpace(token.ToString()))
        {
            sb.AppendLine($"{title} not found.");
        }
        else if (int.TryParse(token.ToString(), out var exp))
        {
            var startDate = new DateTime(1970, 1, 1);
            var expDate = startDate.AddSeconds(exp);
            sb.AppendLine($"{title} = {expDate}");
        }
        else
        {
            sb.AppendLine($"Unable to parse {title}.");
        }
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