using System.Collections.ObjectModel;
using System.Runtime.Intrinsics.X86;
using Avalonia;
using Avalonia.Threading;
using FlowChartMcpServer.ViewModels;
using NodifyM.Avalonia.ViewModelBase;

namespace FlowChartMcpServer.Helper;

public static class ForceDirectedLayout
{
    // 算法参数（可根据实际效果调整）
    private const double RepulsionConstant = 1000; // 斥力系数
    private const double SpringConstant = 0.1;    // 引力系数
    private const double SpringLength = 150;      // 理想连线长度
    private const double Damping = 0.8;           // 阻尼系数
    private const double MaxDisplacement = 10;    // 单步最大位移
    private const int MaxIterations = 500;        // 最大迭代次数

    public static double Length(this Point point)
    {
        return Math.Sqrt(point.X * point.X + point.Y * point.Y);
    }
    
    public static List<Point> ArrangeNodes(
        ObservableCollection<BaseNodeViewModel> nodes,
        ObservableCollection<ConnectionViewModelBase> connections,
        double canvasWidth,
        double canvasHeight)
    {
        
        // 初始化随机位置（如果节点无初始位置）
        InitializeNodePositions(nodes, canvasWidth, canvasHeight);

        // 创建节点索引和连接列表
        var nodeIndexMap = nodes
                           .Select((node, index) => (node, index))
                           .ToDictionary(pair => pair.node, pair => pair.index);
        
        var connectionList = connections.Cast<FlowChartConnectionViewModel>()
                             .Where(conn => nodeIndexMap.ContainsKey(conn.From) && 
                                            nodeIndexMap.ContainsKey(conn.To))
                             .ToList();

        // 创建位置和速度数组
        var positions = nodes.Select(n => n.Location).ToArray();
        var velocities = new Vector[positions.Length];

        // 迭代优化布局
        for (int iter = 0; iter < MaxIterations; iter++)
        {
            var displacements = new Vector[positions.Length];
            double temperature = MaxDisplacement * (1 - (double)iter / MaxIterations);

            // 计算节点间斥力
            for (int i = 0; i < positions.Length; i++)
            {
                for (int j = i + 1; j < positions.Length; j++)
                {
                    var delta = positions[i] - positions[j];
                    double distance = delta.Length() + 0.1; // 避免除零
                    double repulsion = RepulsionConstant / (distance * distance);
                    
                    displacements[i] += delta / distance * repulsion;
                    displacements[j] -= delta / distance * repulsion;
                }
            }

            // 计算连接引力
            foreach (var conn in connectionList)
            {
                int srcIndex = nodeIndexMap[conn.From];
                int tgtIndex = nodeIndexMap[conn.To];
                
                var delta = positions[srcIndex] - positions[tgtIndex];
                double distance = delta.Length() + 0.1;
                double attraction = SpringConstant * Math.Log(distance / SpringLength);
                
                displacements[srcIndex] -= delta / distance * attraction;
                displacements[tgtIndex] += delta / distance * attraction;
            }

            // 更新位置和速度
            for (int i = 0; i < positions.Length; i++)
            {
                // 限制最大位移
                if (displacements[i].Length > temperature)
                {
                    displacements[i] = displacements[i].Normalize() * temperature;
                }
                
                // 更新速度（带阻尼）
                velocities[i] = (velocities[i] + displacements[i]) * Damping;
                
                // 更新位置
                positions[i] += velocities[i];
                
                // 边界约束
                positions[i] = new Point(
                    Math.Max(0, Math.Min(canvasWidth, positions[i].X)),
                    Math.Max(0, Math.Min(canvasHeight, positions[i].Y))
                );
            }
        }

        return positions.ToList();
    }

    private static void InitializeNodePositions(
        ObservableCollection<BaseNodeViewModel> nodes,
        double width,
        double height)
    {
        var random = new Random();
        foreach (var node in nodes)
        {
            // 只初始化未设置位置的节点
            if (node.Location == default)
            {
                node.Location = new Point(
                    random.NextDouble() * width * 0.8 + width * 0.1,
                    random.NextDouble() * height * 0.8 + height * 0.1
                );
            }
        }
    }
}