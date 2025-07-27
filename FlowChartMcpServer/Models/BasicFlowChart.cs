namespace FlowChartMcpServer.Models;

public class BasicFlowChart
{
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public Dictionary<string, FlowChartNode> Nodes { get; set; } = new();

    public List<FlowChartConnection> Connections { get; set; } = new();
}
