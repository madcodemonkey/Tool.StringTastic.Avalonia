using Avalonia.Controls;

namespace Avalonia.StringTastic.Views;

public partial class SorterView : UserControl
{
    public SorterView()
    {
        InitializeComponent();
        DataContext = new ViewModels.SorterViewModel();
    }
}