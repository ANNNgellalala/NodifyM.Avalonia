using Avalonia.Controls;
using FlowChartMcpServer.ViewModels;

namespace FlowChartMcpServer.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext=new MainWindowViewModel();
    }
}
