using Avalonia.Controls;
using Avalonia.StringTastic.ViewModels;

namespace Avalonia.StringTastic.Views;

public partial class JwtDecoderView : UserControl
{
    public JwtDecoderView()
    {
        InitializeComponent();
        DataContext = new JwtDecoderViewModel();
    }
}