using Avalonia.Controls;
using Avalonia.StringTastic.ViewModels;

namespace Avalonia.StringTastic.Views;

public partial class Base64EncoderView : UserControl
{
    public Base64EncoderView()
    {
        InitializeComponent();
        DataContext = new Base64EncoderViewModel();
    }
}