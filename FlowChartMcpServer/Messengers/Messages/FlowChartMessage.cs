using CommunityToolkit.Mvvm.Messaging.Messages;
using FlowChartMcpServer.Models;

namespace FlowChartMcpServer.Messengers.Messages;

public class FlowChartMessage : ValueChangedMessage<BasicFlowChart>
{
    public FlowChartMessage(
        BasicFlowChart value,
        FlowChartMessageType operation
        ) : base(value)
    {
        Operation = operation;
    }

    public FlowChartMessageType Operation { get; set; }

    public List<FlowChartNode> Nodes { get; set; } = [];
    
    public List<FlowChartConnection> Connections { get; set; } = [];
}

public enum FlowChartMessageType
{
    Create,
    AddNodes,
    AddConnections
}
