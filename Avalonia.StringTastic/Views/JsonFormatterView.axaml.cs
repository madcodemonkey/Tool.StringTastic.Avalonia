using Avalonia.Controls;
using Avalonia.StringTastic.ViewModels;

namespace Avalonia.StringTastic.Views;

public partial class JsonFormatterView : UserControl
{
    public JsonFormatterView()
    {
        InitializeComponent();
        DataContext = new JsonFormatterViewModel();
    }
}