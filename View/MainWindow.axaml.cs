using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using Microsoft.Extensions.Hosting;
using Serilog.Sinks.File;
using Serilog.Sinks.Observable;
using Sim.Host;
using Sim.Model;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;
using System.Threading;

namespace Sim.View;

public partial class MainWindow : Window
{
    private readonly WriteableBitmap _bitmap;
    private readonly DispatcherTimer _timer;

    private const int FPS = 60;
    private const int WIDTH = 800;
    private const int HEIGHT = 800;

    private World _world;
    private Sim.Geometry.Point RenderScale = new Sim.Geometry.Point(1.0, 1.0);

    public int ImageWidth => WIDTH;
    public int ImageHeight => HEIGHT;
    public ObservableCollection<string> LogMessages { get; } = [];

    public MainWindow()
    {
        InitializeComponent();

        DataContext = this;

        _bitmap = new WriteableBitmap(new PixelSize(WIDTH, HEIGHT), new Vector(96 * RenderScaling, 96 * RenderScaling), PixelFormat.Bgra8888);
        DisplayImage.Source = _bitmap;

        var host = Sim.Host.HostBuilder.Create()
            /* .ConfigureLogging(s => LogMessages.Add(s)) */
            .ConfigureLogging(s => {})
            .Settings(new WorldSettings(HumansCount: 1000, MapWidth: 800, MapHeight: 800))
            .Build();

        var cts = new CancellationTokenSource();
        Closed += (s, e) => cts.Cancel();
        host.StartAsync(cts.Token);

        _world = (World)host.Services.GetService(typeof(World));

        RenderScale = new Sim.Geometry.Point((double)WIDTH / _world.Size.Width, (double)HEIGHT / _world.Size.Height);

        var interval = TimeSpan.FromSeconds(1) / FPS;
        _timer = new DispatcherTimer { Interval = interval };
        _timer.Tick += Draw;
        _timer.Start();
    }

    private void Draw(object sender, EventArgs e)
    {
        var positions = _world.GetPositions();
        if (positions is null)
            return;

        using var buf = _bitmap.Lock();
        var info = new SKImageInfo(WIDTH, HEIGHT, SKColorType.Bgra8888);
        using var surface = SKSurface.Create(info, buf.Address, buf.RowBytes);

        var canvas = surface.Canvas;

        canvas.Clear(SKColors.Black);
        using var paint = new SKPaint { Color = SKColors.LightGreen };

        foreach (var pos in positions)
        {
            var renderPos = pos.Value * RenderScale;
            canvas.DrawRect((float)renderPos.X, (float)renderPos.Y, (float)(1 * RenderScale.X), (float)(1 * RenderScale.Y), paint);
        }

        DisplayImage.InvalidateVisual();
    }
}