using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.StringTastic.ViewModels;
using Avalonia.VisualTree;

namespace Avalonia.StringTastic.Views;

public partial class ColorPickerView : UserControl
{
    public ColorPickerView()
    {
        InitializeComponent();
        var vm = new ColorPickerViewModel();
        vm.CopyToClipboard = CopyToClipboardAsync;
        vm.OpenColorPicker = OpenColorPickerDialog;
        DataContext = vm;
    }

    private async Task CopyToClipboardAsync(string text)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is not null)
        {
            await topLevel.Clipboard!.SetTextAsync(text);
        }
    }

    private async Task<Color?> OpenColorPickerDialog(Color initialColor)
    {
        var tcs = new TaskCompletionSource<Color?>();

        byte r = initialColor.R, g = initialColor.G, b = initialColor.B;

        var preview = new Border
        {
            MinHeight = 60,
            CornerRadius = new CornerRadius(4),
            Background = new SolidColorBrush(initialColor),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Margin = new Thickness(0, 0, 0, 10),
        };

        var hexLabel = new TextBlock
        {
            Text = $"#{r:X2}{g:X2}{b:X2}",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = 18,
            FontWeight = FontWeight.Bold,
        };

        // Put hex label centered on the preview
        preview.Child = hexLabel;

        var rSlider = new Slider { Minimum = 0, Maximum = 255, Value = r, Margin = new Thickness(0, 5) };
        var gSlider = new Slider { Minimum = 0, Maximum = 255, Value = g, Margin = new Thickness(0, 5) };
        var bSlider = new Slider { Minimum = 0, Maximum = 255, Value = b, Margin = new Thickness(0, 5) };

        var rLabel = new TextBlock { Text = $"Red: {r}", Margin = new Thickness(0, 10, 0, 0) };
        var gLabel = new TextBlock { Text = $"Green: {g}", Margin = new Thickness(0, 10, 0, 0) };
        var bLabel = new TextBlock { Text = $"Blue: {b}", Margin = new Thickness(0, 10, 0, 0) };

        void UpdateColor()
        {
            r = (byte)rSlider.Value;
            g = (byte)gSlider.Value;
            b = (byte)bSlider.Value;
            var color = Color.FromRgb(r, g, b);
            preview.Background = new SolidColorBrush(color);
            hexLabel.Text = $"#{r:X2}{g:X2}{b:X2}";
            rLabel.Text = $"Red: {r}";
            gLabel.Text = $"Green: {g}";
            bLabel.Text = $"Blue: {b}";
        }

        rSlider.ValueChanged += (_, _) => UpdateColor();
        gSlider.ValueChanged += (_, _) => UpdateColor();
        bSlider.ValueChanged += (_, _) => UpdateColor();

        var okButton = new Button
        {
            Content = "OK",
            Width = 80,
            Height = 35,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 15, 0, 0),
        };

        var cancelButton = new Button
        {
            Content = "Cancel",
            Width = 80,
            Height = 35,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(0, 15, 0, 0),
        };

        var buttonPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            Spacing = 15,
            Children = { okButton, cancelButton },
        };

        var panel = new StackPanel
        {
            Margin = new Thickness(15),
            Children =
            {
                preview,
                rLabel, rSlider,
                gLabel, gSlider,
                bLabel, bSlider,
                buttonPanel,
            }
        };

        var dialog = new Window
        {
            Title = "Pick a Color",
            Content = panel,
            Width = 400,
            Height = 420,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
        };

        okButton.Click += (_, _) =>
        {
            tcs.SetResult(Color.FromRgb((byte)rSlider.Value, (byte)gSlider.Value, (byte)bSlider.Value));
            dialog.Close();
        };

        cancelButton.Click += (_, _) =>
        {
            tcs.SetResult(null);
            dialog.Close();
        };

        dialog.Closing += (_, _) =>
        {
            if (!tcs.Task.IsCompleted)
                tcs.SetResult(null);
        };

        var ownerWindow = this.FindAncestorOfType<Window>();
        if (ownerWindow is not null)
        {
            await dialog.ShowDialog(ownerWindow);
        }
        else
        {
            tcs.SetResult(null);
        }

        return await tcs.Task;
    }
}