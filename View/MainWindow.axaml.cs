using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sim.Host;
using System.Linq;
using System.Threading;

namespace Sim.View;

public partial class MainWindow : Window
{
    public int ImageWidth => Renderer.WIDTH;
    public int ImageHeight => Renderer.HEIGHT;

    private Renderer Renderer;
    private InputHandler InputHandler;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        var host = Sim.Host.HostBuilder.Create()
            .UseCommandLineArgs()
            .Build();

        var cts = new CancellationTokenSource();
        Closed += (s, e) => cts.Cancel();
        host.StartAsync(cts.Token);

        var worldHost = host.Services.GetServices<IHostedService>().OfType<IWorldHost>().First();

        var zoomCalc = new ZoomCalculator();
        var panCalc = new PanCalculator();

        SetupRenderer(worldHost, zoomCalc, panCalc);
        SetupInputHandler(worldHost, zoomCalc, panCalc);
    }

    private void SetupRenderer(IWorldHost worldHost, ZoomCalculator zoomCalc, PanCalculator panCalc)
    {
        var renderScale = new Sim.Geometry.Point((double)ImageWidth / worldHost.WorldSize.Width, (double)ImageHeight / worldHost.WorldSize.Height);

        Renderer = new Renderer(RenderScaling, zoomCalc, panCalc)
        {
            GetPositions = worldHost.GetPositions
        };

        Renderer.SetRenderScale(renderScale);
        Renderer.AfterDraw += DisplayImage.InvalidateVisual;

        DisplayImage.Source = Renderer.Bitmap;

        Renderer.Run();
    }

    private void SetupInputHandler(IWorldHost worldHost, ZoomCalculator zoomCalc, PanCalculator panCalc)
    {
        InputHandler = new InputHandler(this, worldHost, zoomCalc, panCalc);
        KeyDown += (_, e) => InputHandler.Handle(e);
    }
}
