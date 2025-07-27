using Avalonia;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.Input;
using NodifyM.Avalonia.ViewModelBase;

namespace FlowChartMcpServer.ViewModels;

public partial class MainWindowViewModel : NodifyEditorViewModelBase
{
    public MainWindowViewModel()
    {
        Nodes.Add(new KnotNodeViewModel()
        {
            Location = new Point(50, 50)
        });
        Nodes.Add(new KnotNodeViewModel()
        {
            Location = new Point(100, 50)
        });
    }
    
    public override void Connect(ConnectorViewModelBase source, ConnectorViewModelBase target)
    {
        base.Connect(source, target);
    }

    public override void DisconnectConnector(ConnectorViewModelBase connector)
    {
        base.DisconnectConnector(connector);
    }
    [RelayCommand]
    private void ChangeTheme()
    {
        Application.Current!.RequestedThemeVariant = Application.Current!.ActualThemeVariant==ThemeVariant.Dark ? ThemeVariant.Light : ThemeVariant.Dark;
    }
}
