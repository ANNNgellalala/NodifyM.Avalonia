using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Messaging;
using FlowChartMcpServer.Messengers.Messages;
using FlowChartMcpServer.ViewModels;
using NodifyM.Avalonia.Controls;

namespace FlowChartMcpServer.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
}
