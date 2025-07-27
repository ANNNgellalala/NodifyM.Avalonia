using System.ComponentModel;
using System.Text.Json.Serialization;

namespace FlowChartMcpServer.Models;

public class FlowChartNode
{
    [Description("节点名称")]
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [Description("节点描述")]
    [JsonPropertyName("description")]
    public string Description { get; set; }
}
