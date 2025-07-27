using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;
using FlowChartMcpServer.Models;
using NodifyM.Avalonia.ViewModelBase;

namespace FlowChartMcpServer.ViewModels;

public partial class FlowChartNodeViewModel : NodeViewModelBase
{
    public FlowChartNodeViewModel(
        FlowChartNode node)
    {
        Title = node.Id;
        Input.Add(new FlowChartConnectorViewModel(ConnectorViewModelBase.ConnectorFlow.Input, this));
        Output.Add(new FlowChartConnectorViewModel(ConnectorViewModelBase.ConnectorFlow.Output, this));
    }
}
