using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Styling;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FlowChartMcpServer.Helper;
using FlowChartMcpServer.Messengers.Messages;
using FlowChartMcpServer.Models;
using NodifyM.Avalonia.ViewModelBase;
using OpenTelemetry.Trace;

namespace FlowChartMcpServer.ViewModels;

public partial class MainWindowViewModel : ObservableObject, IRecipient<FlowChartMessage>
{
    public IServiceProvider ServiceProvider { get; set; }

    [ObservableProperty]
    private ObservableCollection<FlowChartViewModel> _chartList;

    [ObservableProperty]
    private FlowChartViewModel _selectedChart;

    public MainWindowViewModel(
        IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        var chart = new FlowChartViewModel();
        ChartList = [chart];
        SelectedChart = chart;
        WeakReferenceMessenger.Default.Register(this);
    }

    [RelayCommand]
    private void ChangeTheme()
    {
        Application.Current!.RequestedThemeVariant = Application.Current!.ActualThemeVariant == ThemeVariant.Dark ? ThemeVariant.Light : ThemeVariant.Dark;
    }

    public void Receive(
        FlowChartMessage message)
    {
        var chart = message.Value;
        switch (message.Operation)
        {
            case FlowChartMessageType.Create:
            {
                var existed = ChartList.FirstOrDefault(item => item.Name == chart.Name);
                if (existed is null)
                {
                    var flowChart = new FlowChartViewModel(chart);
                    ChartList.Add(flowChart);
                }
                else
                {
                    ChartList.Remove(existed);
                    ChartList.Add(new FlowChartViewModel(chart));
                }

                break;
            }
            case FlowChartMessageType.AddNodes:
            {
                var flowChart = ChartList.First(item => item.Name == chart.Name);
                foreach (var flowChartNode in message.Nodes)
                    flowChart.Nodes.Add(new FlowChartNodeViewModel(flowChartNode));
                break;
            }
            case FlowChartMessageType.AddConnections:
            {
                var flowChart = ChartList.First(item => item.Name == chart.Name);
                var dictionary = flowChart.Nodes.Cast<FlowChartNodeViewModel>().ToDictionary(node => node.Title);

                foreach (var connection in message.Connections)
                {
                    var source = dictionary[connection.FromNodeId];
                    var target = dictionary[connection.ToNodeId];
                    flowChart.Connections.Add(new FlowChartConnectionViewModel(flowChart,
                        source.Output[0],
                        target.Input[0],
                        connection.Description,
                        source,
                        target));
                }

                break;
            }
        }
        
        var c = ChartList.First(item => item.Name == chart.Name);
        Task.Run(() =>
        {
            var positions = ForceDirectedLayout.ArrangeNodes(c.Nodes, c.Connections, 800, 600);
            for (var i = 0; i < c.Nodes.Count; i++)
                c.Nodes[i].Location = positions[i];
        }).ConfigureAwait(false);
    }
}
