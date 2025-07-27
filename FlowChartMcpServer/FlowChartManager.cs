using FlowChartMcpServer.Models;

namespace FlowChartMcpServer;

public class FlowChartManager
{
    public Dictionary<string, BasicFlowChart> FlowCharts { get; } = new();
}
