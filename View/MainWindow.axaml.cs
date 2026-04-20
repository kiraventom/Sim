using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog.Sinks.File;
using Serilog.Sinks.Observable;
using Sim.Host;
using Sim.Model;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace Sim.View;

public partial class MainWindow : Window
{
    private WriteableBitmap _bitmap;
    private DispatcherTimer _timer;

    private const int FPS = 60;
    private const int WIDTH = 800;
    private const int HEIGHT = 800;

    private WorldHost _worldHost;
    private Sim.Geometry.Point RenderScale = new Sim.Geometry.Point(1.0, 1.0);

    public int ImageWidth => WIDTH;
    public int ImageHeight => HEIGHT;
    public ObservableCollection<string> LogMessages { get; } = [];

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        _bitmap = new WriteableBitmap(new PixelSize(WIDTH, HEIGHT), new Vector(96 * RenderScaling, 96 * RenderScaling), PixelFormat.Bgra8888);
        DisplayImage.Source = _bitmap;

        var host = Sim.Host.HostBuilder.Create()
            /* .ConfigureLogging(s => LogMessages.Add(s)) */
            .ConfigureLogging(s => {})
            .UseCommandLineArgs()
            .Build();

        var cts = new CancellationTokenSource();
        Closed += (s, e) => cts.Cancel();
        host.StartAsync(cts.Token);

        _worldHost = host.Services.GetServices<IHostedService>().OfType<WorldHost>().First();

        RenderScale = new Sim.Geometry.Point((double)WIDTH / _worldHost.WorldSize.Width, (double)HEIGHT / _worldHost.WorldSize.Height);

        var interval = TimeSpan.FromSeconds(1) / FPS;
        _timer = new DispatcherTimer { Interval = interval };
        _timer.Tick += Draw;
        _timer.Start();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.C when e.KeyModifiers.HasFlag(KeyModifiers.Control):
                Close();
                return;

            case Key.Space:
                _worldHost.TogglePause();
                return;
        }
    }

    private void Draw(object sender, EventArgs e)
    {
        var positions = _worldHost.GetPositions();
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