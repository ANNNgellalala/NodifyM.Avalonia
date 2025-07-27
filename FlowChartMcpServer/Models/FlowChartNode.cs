using System.ComponentModel;

namespace FlowChartMcpServer.Models;

public class FlowChartNode
{
    [Description("节点名称")]
    public string Id { get; set; }
    
    [Description("节点描述")]
    public string Description { get; set; }
}
