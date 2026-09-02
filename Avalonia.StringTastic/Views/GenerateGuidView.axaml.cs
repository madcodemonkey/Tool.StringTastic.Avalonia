using Avalonia.Controls;
using Avalonia.StringTastic.ViewModels;

namespace Avalonia.StringTastic.Views;

public partial class GenerateGuidView : UserControl
{
    public GenerateGuidView()
    {
        InitializeComponent();
        DataContext = new GenerateGuidViewModel();
    }
}