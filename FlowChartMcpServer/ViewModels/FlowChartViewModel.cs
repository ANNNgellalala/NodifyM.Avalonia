using CommunityToolkit.Mvvm.ComponentModel;
using FlowChartMcpServer.Models;
using NodifyM.Avalonia.ViewModelBase;

namespace FlowChartMcpServer.ViewModels;

public partial class FlowChartViewModel : NodifyEditorViewModelBase
{
    [ObservableProperty]
    private string _name;
    
    [ObservableProperty]
    private string _description;

    public FlowChartViewModel()
    {
        Name = "New Flow Chart";
        Description = "Description of the flow chart";
    }
    
    public FlowChartViewModel(
        BasicFlowChart chart)
    {
        Name = chart.Name;
        Description = chart.Description;
        var dictionary = new Dictionary<string, FlowChartNodeViewModel>();
        foreach (var node in chart.Nodes)
        {
            var nodeView = new FlowChartNodeViewModel(node.Value);
            Nodes.Add(nodeView);
            dictionary[node.Key] = nodeView;
        }
        
        foreach (var connection in chart.Connections)
        {
            var source = dictionary[connection.FromNodeId];
            var target = dictionary[connection.ToNodeId];
            Connections.Add(new FlowChartConnectionViewModel(this,
                source.Output[0],
                target.Input[0],
                connection.Description,
                source,
                target));
        }
    }
}
