using System.ComponentModel;

namespace FlowChartMcpServer.Models;

public class FlowChartConnection
{
    [Description("连接ID")]
    public string FromNodeId { get; set; }
    
    [Description("连接目标ID")]
    public string ToNodeId { get; set; }
    
    [Description("连接描述")]
    public string Description { get; set; }
}
