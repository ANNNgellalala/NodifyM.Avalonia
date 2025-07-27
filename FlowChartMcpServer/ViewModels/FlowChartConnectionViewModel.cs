using System.ComponentModel;
using NodifyM.Avalonia.ViewModelBase;

namespace FlowChartMcpServer.ViewModels;

public partial class FlowChartConnectionViewModel : ConnectionViewModelBase
{
    public FlowChartConnectionViewModel(
        NodifyEditorViewModelBase nodifyEditor,
        ConnectorViewModelBase source,
        ConnectorViewModelBase target,
        NodeViewModelBase from,
        NodeViewModelBase to) : base(nodifyEditor, source, target)
    {
        From = from;
        To = to;
    }

    public FlowChartConnectionViewModel(
        NodifyEditorViewModelBase nodifyEditor,
        ConnectorViewModelBase source,
        ConnectorViewModelBase target,
        string text,
        NodeViewModelBase from,
        NodeViewModelBase to) : base(nodifyEditor, source, target, text)
    {
        From = from;
        To = to;
    }

    public NodeViewModelBase From { get; set; }

    public NodeViewModelBase To { get; set; }
}
