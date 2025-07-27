using System.ComponentModel;
using System.Text.Json.Serialization;

namespace FlowChartMcpServer.Models;

public class FlowChartConnection
{
    [Description("连接ID")]
    [JsonPropertyName("fromNodeId")]
    public string FromNodeId { get; set; }
    
    [Description("连接目标ID")]
    [JsonPropertyName("toNodeId")]
    public string ToNodeId { get; set; }
    
    [Description("连接描述")]
    [JsonPropertyName("description")]
    public string Description { get; set; }
}
