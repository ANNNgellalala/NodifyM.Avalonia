// See https://aka.ms/new-console-template for more information

using System.ClientModel;
using System.ClientModel.Primitives;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using OpenAI;

Console.WriteLine("Hello, World!");


// Connect to an MCP server
Console.WriteLine("Connecting client to MCP 'everything' server");

// Create OpenAI client (or any other compatible with IChatClient)
// Provide your own OPENAI_API_KEY via an environment variable.
var openAiClient = new OpenAIClient(new ApiKeyCredential("ssh-ed25519 AAAAC3NzaC1lZDI1NTE5AAAAIDmYdN7lz1Pgf2mLTTcQxsrblMMWKaq5Ni4nlFOJ9wAR"), new OpenAIClientOptions()
{
    Endpoint = new Uri("http://127.0.0.1:11434/v1"),
}).GetChatClient("qwen3:14b");

// Create a sampling client.
using var samplingClient = openAiClient.AsIChatClient()
                                       .AsBuilder()
                                       .Build();

var mcpClient = await McpClientFactory.CreateAsync(new SseClientTransport(new SseClientTransportOptions()
{
    Endpoint = new Uri("http://localhost:5000"),
}));
    
// Get all available tools
Console.WriteLine("Tools available:");
var tools = await mcpClient.ListToolsAsync();
foreach (var tool in tools)
{
    Console.WriteLine(tool.Name);
    Console.WriteLine($"  Description: {tool.Description}");
    Console.WriteLine($"  Input Schema: {tool.JsonSchema.ToString()}");
}

Console.WriteLine();
