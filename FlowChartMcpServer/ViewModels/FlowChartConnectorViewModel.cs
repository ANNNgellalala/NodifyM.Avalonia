using NodifyM.Avalonia.ViewModelBase;

namespace FlowChartMcpServer.ViewModels;

public class FlowChartConnectorViewModel : ConnectorViewModelBase
{
    public NodeViewModelBase Parent { get; set; }

    public FlowChartConnectorViewModel(
        ConnectorFlow flow,
        NodeViewModelBase parent)
    {
        Parent = parent;
        Flow = flow;
    }
}
