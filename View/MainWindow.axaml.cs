using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sim.Host;
using System;
using System.Linq;
using System.Threading;

namespace Sim.View;

public partial class MainWindow : Window
{
    public int MapWidth => 800;
    public int MapHeight => 800;

    public int MiniMapWidth => 300;
    public int MiniMapHeight => 300;

    private MapRenderer MapRenderer;
    private MiniMapRenderer MiniMapRenderer;

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

        SetupRenderers(worldHost);
        SetupInputHandler(worldHost);
    }

    private void SetupRenderers(IWorldHost worldHost)
    {
        var zoomCalc = new ZoomCalculator();
        var panCalc = new PanCalculator();

        // Map
        MapRenderer = new MapRenderer(MapWidth, MapHeight, RenderScaling, zoomCalc, panCalc)
        {
            GetObjects = worldHost.GetObjects,
        };

        InitRenderer(MapRenderer, worldHost, MapImage.InvalidateVisual);
        MapImage.Source = MapRenderer.Bitmap;
        MapRenderer.Run();

        // MiniMap
        MiniMapRenderer = new MiniMapRenderer(MiniMapWidth, MiniMapHeight, RenderScaling, zoomCalc, panCalc)
        {
            GetObjects = worldHost.GetObjects,
        };

        InitRenderer(MiniMapRenderer, worldHost, MiniMapImage.InvalidateVisual);
        MiniMapImage.Source = MiniMapRenderer.Bitmap;
        MiniMapRenderer.Run();
    }

    private static void InitRenderer(Renderer renderer, IWorldHost worldHost, Action afterDraw)
    {
        var renderScale = new Sim.Geometry.Point((double)renderer.Width / worldHost.WorldSize.Width, (double)renderer.Height / worldHost.WorldSize.Height);
        renderer.SetRenderScale(renderScale);
        renderer.AfterDraw += afterDraw;
    }

    private void SetupInputHandler(IWorldHost worldHost)
    {
        InputHandler = new InputHandler(this, worldHost, MapRenderer);
        KeyDown += (_, e) => InputHandler.Handle(e);
    }
}
