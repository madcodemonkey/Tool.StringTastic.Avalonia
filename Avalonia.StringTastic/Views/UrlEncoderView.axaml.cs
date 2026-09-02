using Avalonia.Controls;
using Avalonia.StringTastic.ViewModels;

namespace Avalonia.StringTastic.Views;

public partial class UrlEncoderView : UserControl
{
    public UrlEncoderView()
    {
        InitializeComponent();
        DataContext = new UrlEncoderViewModel();
    }
}