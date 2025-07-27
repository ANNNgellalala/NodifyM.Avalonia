using System.Text.Json;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace FlowChartMcpServer.Tools;

public class McpRequestHandler
{
    public static async ValueTask<CallToolResult> HandleCallToolAsync(
        RequestContext<CallToolRequestParams> context,
        CancellationToken cancellation)
    {
        var result = new CallToolResult();
        var toolName = context.Params!.Name;
        var str = toolName switch
        {
            "create_flow_chart" => await FlowChartTools.CreateFlowChartAsync(context.Server,
                context.Params.Arguments["name"].GetString(),
                context.Params.Arguments["description"].GetString()),
            "add_node" => await FlowChartTools.AddNodeAsync(context.Server,
                context.Params.Arguments["flowChartName"].GetString(),
                context.Params.Arguments["nodeName"].GetString(),
                context.Params.Arguments["nodeDescription"].GetString()),
            "add_connection" => await FlowChartTools.AddConnectionAsync(context.Server,
                context.Params.Arguments["flowChartName"].GetString(),
                context.Params.Arguments["fromNodeName"].GetString(),
                context.Params.Arguments["toNodeName"].GetString(),
                context.Params.Arguments["connectionDescription"].GetString()),
            "get_flow_chart_info" => await FlowChartTools.GetFlowChartInfoAsync(context.Server, context.Params.Arguments["flowChartName"].GetString()),
            _ => null
        };
        if (str is null)
        {
            result.IsError = true;
            result.Content = new List<ContentBlock>()
            {
                new TextContentBlock
                {
                    Text = $"工具 '{toolName}' 不存在或未实现。"
                }
            };
            return result;
        }
        
        result.IsError = false;
        result.Content = new List<ContentBlock>
        {
            new TextContentBlock
            {
                Text = str
            }
        };
        return result;
    }

    public static ValueTask<ListToolsResult> HandleListToolsAsync(
        RequestContext<ListToolsRequestParams> context,
        CancellationToken cancellation)
    {
        var tools = new List<Tool>();
        tools.Add(new Tool
        {
            Name = "create_flow_chart",
            Description = "初始化创建一个新的流程图",
            InputSchema = JsonDocument.Parse("""
                                             {
                                                 "type": "object",
                                                 "properties": {
                                                     "name": {
                                                         "type": "string",
                                                         "description": "流程图名称"
                                                     },
                                                     "description": {
                                                         "type": "string",
                                                         "description": "流程图描述"
                                                     }
                                                 },
                                                 "required": ["name", "description"]
                                             }
                                             """)
                                      .RootElement
        });
        tools.Add(new Tool
        {
            Name = "add_node",
            Description = "在流程图中添加节点",
            InputSchema = JsonDocument.Parse("""
                                             {
                                                 "type": "object",
                                                 "properties": {
                                                     "flowChartName": {
                                                         "type": "string",
                                                         "description": "流程图名称"
                                                     },
                                                     "nodeName": {
                                                         "type": "string",
                                                         "description": "节点名称"
                                                     },
                                                     "nodeDescription": {
                                                         "type": "string",
                                                         "description": "节点描述"
                                                     }
                                                 },
                                                 "required": ["flowChartName", "nodeName", "nodeDescription"]
                                             }
                                             """)
                                      .RootElement
        });
        tools.Add(new Tool
        {
            Name = "add_connection",
            Description = "在流程图中添加连接",
            InputSchema = JsonDocument.Parse("""
                                             {
                                                 "type": "object",
                                                 "properties": {
                                                     "flowChartName": {
                                                         "type": "string",
                                                         "description": "流程图名称"
                                                     },
                                                     "fromNodeName": {
                                                         "type": "string",
                                                         "description": "起始节点名称"
                                                     },
                                                     "toNodeName": {
                                                         "type": "string",
                                                         "description": "目标节点名称"
                                                     },
                                                     "connectionDescription": {
                                                         "type": "string",
                                                         "description": "连接描述"
                                                     }
                                                 },
                                                 "required": ["flowChartName", "fromNodeName", "toNodeName"]
                                             }
                                             """)
                                      .RootElement
        });
        tools.Add(new Tool
        {
            Name = "get_flow_chart_info",
            Description = "获取流程图的详细信息",
            InputSchema = JsonDocument.Parse("""
                                             {
                                                 "type": "object",
                                                 "properties": {
                                                     "flowChartName": {
                                                         "type": "string",
                                                         "description": "流程图名称"
                                                     }
                                                 },
                                                 "required": ["flowChartName"]
                                             }
                                             """)
                                      .RootElement
        });

        return ValueTask.FromResult(new ListToolsResult
        {
            Tools = tools
        });
    }
}
