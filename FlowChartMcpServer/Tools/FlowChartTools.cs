using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using CommunityToolkit.Mvvm.Messaging;
using FlowChartMcpServer.Messengers.Messages;
using FlowChartMcpServer.Models;
using ModelContextProtocol.Server;

namespace FlowChartMcpServer.Tools;

[McpServerToolType]
public class FlowChartTools
{
    [McpServerTool]
    [Description("初始化创建一个新的流程图")]
    public static async Task<string> CreateFlowChartAsync(
        IMcpServer server,
        [Description("流程图名称")] string name,
        [Description("流程图描述")] string description)
    {
        var flowChartManager = server.Services!.GetRequiredService<FlowChartManager>();
        var logger = server.Services!.GetRequiredService<ILogger<FlowChartTools>>();
        if (flowChartManager.FlowCharts.ContainsKey(name))
        {
            return $"流程图 '{name}' 已存在，请选择一个不同的名称。";
        }

        var flowChart = new BasicFlowChart
        {
            Name = name, Description = description
        };
        WeakReferenceMessenger.Default.Send(new FlowChartMessage(flowChart, FlowChartMessageType.Create));
        flowChartManager.FlowCharts.Add(name, flowChart);
        logger.LogInformation("创建流程图 '{Name}'", name);
        return await Task.FromResult($"流程图 '{name}' 已创建。请记住流程图名称{name}，以便后续操作。");
    }

    [McpServerTool]
    [Description("在流程图中添加节点")]
    public static async Task<string> AddNodeAsync(
        IMcpServer server,
        [Description("流程图名称")] string flowChartName,
        [Description("节点名称")] string nodeName,
        [Description("节点描述")] string nodeDescription)
    {
        var flowChartManager = server.Services!.GetRequiredService<FlowChartManager>();
        var logger = server.Services!.GetRequiredService<ILogger<FlowChartTools>>();
        if (!flowChartManager.FlowCharts.TryGetValue(flowChartName, out var flowChart))
        {
            return $"流程图 '{flowChartName}' 不存在。请先创建流程图。";
        }

        var node = new FlowChartNode
        {
            Id = nodeName, Description = nodeDescription
        };

        flowChart.Nodes.Add(node.Id, node);
        var message = new FlowChartMessage(flowChart, FlowChartMessageType.AddNodes);
        message.Nodes = [node];
        WeakReferenceMessenger.Default.Send(message);
        logger.LogInformation("在流程图 '{FlowChartName}' 中添加节点 '{NodeName}'", flowChartName, nodeName);
        return await Task.FromResult($"节点 '{nodeName}' 已添加到流程图 '{flowChartName}'。");
    }
    
    [McpServerTool]
    [Description("在流程图中添加多个节点")]
    public static async Task<string> AddBatchNodesAsync(
        IMcpServer server,
        [Description("流程图名称")] string flowChartName,
        [Description("节点列表（JSON格式）")] List<FlowChartNode> nodes)
    {
        var flowChartManager = server.Services!.GetRequiredService<FlowChartManager>();
        var logger = server.Services!.GetRequiredService<ILogger<FlowChartTools>>();
        if (!flowChartManager.FlowCharts.TryGetValue(flowChartName, out var flowChart))
        {
            return $"流程图 '{flowChartName}' 不存在。请先创建流程图。";
        }
        
        if (nodes.Count == 0)
        {
            return "节点列表为空。请提供有效的节点列表。";
        }

        var successfulAdditions = new List<FlowChartNode>();
        foreach (var node in nodes)
        {
            if (!flowChart.Nodes.TryAdd(node.Id, node))
            {
                logger.LogWarning("节点 '{NodeId}' 已存在于流程图 '{FlowChartName}'，跳过添加。", node.Id, flowChartName);
                continue;
            }

            successfulAdditions.Add(node);
            logger.LogInformation("在流程图 '{FlowChartName}' 中添加节点 '{NodeId}'", flowChartName, node.Id);
        }

        var message = new FlowChartMessage(flowChart, FlowChartMessageType.AddNodes);
        message.Nodes = successfulAdditions;
        WeakReferenceMessenger.Default.Send(message);
        return await Task.FromResult($"已添加 {nodes.Count} 个节点到流程图 '{flowChartName}'，成功添加的节点有：{String.Join(", ", successfulAdditions.Select(item => item.Id))}。");
    }

    [McpServerTool]
    [Description("在流程图中添加连接")]
    public static async Task<string> AddConnectionAsync(
        IMcpServer server,
        [Description("流程图名称")] string flowChartName,
        [Description("起始节点名称")] string fromNodeName,
        [Description("目标节点名称")] string toNodeName,
        [Description("连接描述"), Optional] string connectionDescription)
    {
        var flowChartManager = server.Services!.GetRequiredService<FlowChartManager>();
        var logger = server.Services!.GetRequiredService<ILogger<FlowChartTools>>();
        if (!flowChartManager.FlowCharts.TryGetValue(flowChartName, out var flowChart))
        {
            return $"流程图 '{flowChartName}' 不存在。请先创建流程图。";
        }

        if (!flowChart.Nodes.ContainsKey(fromNodeName) || !flowChart.Nodes.ContainsKey(toNodeName))
        {
            return $"节点 '{fromNodeName}' 或 '{toNodeName}' 不存在。请先添加节点。";
        }

        var connection = new FlowChartConnection
        {
            FromNodeId = fromNodeName, ToNodeId = toNodeName, Description = connectionDescription
        };
        flowChart.Connections.Add(connection);
        var message = new FlowChartMessage(flowChart, FlowChartMessageType.AddConnections);
        message.Connections = [connection];
        WeakReferenceMessenger.Default.Send(message);
        logger.LogInformation("在流程图 '{FlowChartName}' 中添加连接从 '{FromNodeName}' 到 '{ToNodeName}'", flowChartName, fromNodeName, toNodeName);
        return await Task.FromResult($"连接从 '{fromNodeName}' 到 '{toNodeName}' 已添加到流程图 '{flowChartName}'。");
    }
    
    [McpServerTool]
    [Description("在流程图中添加多个连接")]
    public static async Task<string> AddBatchConnectionsAsync(
        IMcpServer server,
        [Description("流程图名称")] string flowChartName,
        [Description("连接列表（JSON格式）")] List<FlowChartConnection> connections)
    {
        var flowChartManager = server.Services!.GetRequiredService<FlowChartManager>();
        var logger = server.Services!.GetRequiredService<ILogger<FlowChartTools>>();
        if (!flowChartManager.FlowCharts.TryGetValue(flowChartName, out var flowChart))
        {
            return $"流程图 '{flowChartName}' 不存在。请先创建流程图。";
        }
        
        if (connections.Count == 0)
        {
            return "连接列表为空。请提供有效的连接列表。";
        }

        var successfulAdditions = new List<FlowChartConnection>();
        foreach (var connection in connections)
        {
            if (!flowChart.Connections.Contains(connection))
            {
                flowChart.Connections.Add(connection);
                successfulAdditions.Add(connection);
                logger.LogInformation("在流程图 '{FlowChartName}' 中添加连接从 '{FromNodeId}' 到 '{ToNodeId}'", flowChartName, connection.FromNodeId, connection.ToNodeId);
            }
            else
            {
                logger.LogWarning("连接从 '{FromNodeId}' 到 '{ToNodeId}' 已存在于流程图 '{FlowChartName}'，跳过添加。", connection.FromNodeId, connection.ToNodeId, flowChartName);
            }
        }

        var message = new FlowChartMessage(flowChart, FlowChartMessageType.AddConnections);
        message.Connections = successfulAdditions;
        WeakReferenceMessenger.Default.Send(message);
        return await Task.FromResult($"已添加 {connections.Count} 个连接到流程图 '{flowChartName}'，成功添加的连接有：{String.Join(", ", successfulAdditions.Select(item => $"{item.FromNodeId} -> {item.ToNodeId}"))}。");
    }

    [McpServerTool]
    [Description("获取流程图信息")]
    public static async Task<string> GetFlowChartInfoAsync(
        IMcpServer server,
        [Description("流程图名称")] string flowChartName)
    {
        var flowChartManager = server.Services!.GetRequiredService<FlowChartManager>();
        if (!flowChartManager.FlowCharts.TryGetValue(flowChartName, out var flowChart))
        {
            return $"流程图 '{flowChartName}' 不存在。请先创建流程图。";
        }

        var chart = flowChartManager.FlowCharts[flowChartName];
        var info = $"""
                    ```FlowChart
                    流程图： {flowChart.Name}
                    描述： {flowChart.Description}
                    节点数： {flowChart.Nodes.Count}
                    连接数： {flowChart.Connections.Count}
                    节点列表：
                    {
                        String.Join("\n",
                            from node in chart.Nodes
                            orderby node.Key
                            select $"{node.Key}: {node.Value.Description}")
                    }
                    连接列表：
                    {
                        String.Join("\n",
                            from connection in chart.Connections
                            select $"{connection.FromNodeId} -> {connection.ToNodeId}: {connection.Description}"
                        )}
                    ```
                    """;
        return await Task.FromResult(info);
    }
}
