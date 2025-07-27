// See https://aka.ms/new-console-template for more information

using System.ClientModel;
using System.ClientModel.Primitives;
using System.Text.Json.Serialization;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using OpenAI;

Console.WriteLine("Hello, World!");


// Connect to an MCP server
Console.WriteLine("Connecting client to MCP 'everything' server");

// Create OpenAI client (or any other compatible with IChatClient)
// Provide your own OPENAI_API_KEY via an environment variable.
var openAiClient = new OpenAIClient(new ApiKeyCredential("ssh-ed25519 AAAAC3NzaC1lZDI1NTE5AAAAIDmYdN7lz1Pgf2mLTTcQxsrblMMWKaq5Ni4nlFOJ9wAR"),
    new OpenAIClientOptions()
    { Endpoint = new Uri("http://127.0.0.1:11434/v1"), }).GetChatClient("qwen3:14b");

// Create a sampling client.
using var samplingClient = openAiClient.AsIChatClient().AsBuilder().Build();

var mcpClient = await McpClientFactory.CreateAsync(new SseClientTransport(new SseClientTransportOptions()
{ Endpoint = new Uri("http://localhost:5000"), }));

Console.WriteLine("Tools available:");
var tools = await mcpClient.ListToolsAsync();
foreach (var tool in tools)
{
    Console.WriteLine(tool.Name);
    Console.WriteLine($"  Description: {tool.Description}");
    Console.WriteLine($"  Input Schema: {tool.JsonSchema.ToString()}");
}

Console.WriteLine();

var name = Guid.NewGuid().ToString();
// 创建流程图main
var toolName = "create_flow_chart";
await mcpClient.CallToolAsync(toolName,
    new Dictionary<string, object?>()
    { { "name", name },
      { "description", "test" } });

toolName = "add_batch_nodes";
var result = await mcpClient.CallToolAsync(toolName,
    new Dictionary<string, object?>()
    { { "flowChartName", name },
      { "nodes", System.Text.Json.JsonSerializer.SerializeToElement(System.Text.Json.JsonSerializer.Deserialize<List<FlowChartNode>>("""
            [
              {
                "id": "init_params",
                "description": "初始化参数：设置采样周期，创建IMUProcessor和PIDController实例"
              },
              {
                "id": "simulate_imu",
                "description": "模拟IMU数据输入：定义IMUData结构体并赋值"
              },
              {
                "id": "set_target",
                "description": "设置目标姿态：定义目标横滚角和俯仰角"
              },
              {
                "id": "loop_start",
                "description": "控制循环开始：i=0"
              },
              {
                "id": "update_imu",
                "description": "更新IMU数据并解算姿态：调用imu_processor.update()和getEulerAngles()"
              },
              {
                "id": "compute_output",
                "description": "计算控制输出：调用PID控制器的compute()方法"
              },
              {
                "id": "output_result",
                "description": "输出结果：打印当前姿态和控制输出"
              },
              {
                "id": "loop_condition",
                "description": "循环条件判断：i < 100"
              },
              {
                "id": "loop_end",
                "description": "循环结束：return 0"
              }
            ]
            """)) } });
Console.WriteLine((result.Content.First() as TextContentBlock)!.Text);
// Add edges between nodes
toolName = "add_batch_connections";
await mcpClient.CallToolAsync(toolName,
    new Dictionary<string, object?>()
    { { "flowChartName", name },
      { "connections", System.Text.Json.JsonSerializer.SerializeToElement(System.Text.Json.JsonSerializer.Deserialize<List<FlowChartConnection>>("""
            [
              {
                "fromNodeId": "init_params",
                "toNodeId": "simulate_imu",
                "description": "初始化完成后模拟IMU数据"
              },
              {
                "fromNodeId": "simulate_imu",
                "toNodeId": "set_target",
                "description": "模拟IMU数据后设置目标姿态"
              },
              {
                "fromNodeId": "set_target",
                "toNodeId": "loop_start",
                "description": "设置目标姿态后开始控制循环"
              },
              {
                "fromNodeId": "loop_start",
                "toNodeId": "update_imu",
                "description": "循环开始，更新IMU数据"
              },
              {
                "fromNodeId": "update_imu",
                "toNodeId": "compute_output",
                "description": "IMU数据更新后计算控制输出"
              },
              {
                "fromNodeId": "compute_output",
                "toNodeId": "output_result",
                "description": "计算控制输出后输出结果"
              },
              {
                "fromNodeId": "output_result",
                "toNodeId": "loop_condition",
                "description": "输出结果后检查循环条件"
              },
              {
                "fromNodeId": "loop_condition",
                "toNodeId": "update_imu",
                "description": "条件满足，继续循环"
              },
              {
                "fromNodeId": "loop_condition",
                "toNodeId": "loop_end",
                "description": "条件不满足，结束循环"
              }
            ]
            """)) } });

public class FlowChartNode
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }
}

public class FlowChartConnection
{
    [JsonPropertyName("fromNodeId")]
    public string FromNodeId { get; set; }

    [JsonPropertyName("toNodeId")]
    public string ToNodeId { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }
}
