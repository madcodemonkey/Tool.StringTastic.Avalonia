using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Avalonia.StringTastic.ViewModels
{
    public partial class JsonFormatterViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _inputText = string.Empty;

        [RelayCommand]
        private void Format()
        {
            try
            {
                var json = JsonConvert.DeserializeObject(InputText);
                InputText = JsonConvert.SerializeObject(json, Formatting.Indented);
            }
            catch (Exception ex)
            {
                InputText = $"// Invalid JSON: {ex.Message}";
            }
        }

        [RelayCommand]
        private void SortProperties()
        {
            try
            {
                var json = JsonConvert.DeserializeObject(InputText);
                if (json is JObject jObject)
                {
                    InputText = JsonConvert.SerializeObject(SortJObject(jObject), Formatting.Indented);
                }
                else if (json is JArray jArray)
                {
                    InputText = JsonConvert.SerializeObject(SortJArray(jArray), Formatting.Indented);
                }
                else
                {
                    InputText = JsonConvert.SerializeObject(json, Formatting.Indented);
                }
            }
            catch (Exception ex)
            {
                InputText = $"// Invalid JSON: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task Copy()
        {
            if (string.IsNullOrEmpty(InputText))
                return;

            var clipboard = GetClipboard();
            if (clipboard is not null)
            {
                await clipboard.SetTextAsync(InputText);
            }
        }

        [RelayCommand]
        private void Clear()
        {
            InputText = string.Empty;
        }

        private static IClipboard? GetClipboard()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                return desktop.MainWindow?.Clipboard;
            }

            return null;
        }

        private static JObject SortJObject(JObject original)
        {
            var result = new JObject();
            foreach (var property in original.Properties().OrderBy(p => p.Name))
            {
                if (property.Value is JObject jObject)
                {
                    result[property.Name] = SortJObject(jObject);
                }
                else if (property.Value is JArray jArray)
                {
                    result[property.Name] = SortJArray(jArray);
                }
                else
                {
                    result[property.Name] = property.Value;
                }
            }

            return result;
        }

        private static JArray SortJArray(JArray original)
        {
            var result = new JArray();
            foreach (var item in original)
            {
                if (item is JObject jObject)
                {
                    result.Add(SortJObject(jObject));
                }
                else if (item is JArray jArray)
                {
                    result.Add(SortJArray(jArray));
                }
                else
                {
                    result.Add(item);
                }
            }

            return result;
        }
    }
}