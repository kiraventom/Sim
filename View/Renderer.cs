using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;
using Positions = System.Collections.Generic.IReadOnlyDictionary<int, Sim.Geometry.PointI>;

namespace Sim.View;

public class Renderer
{
    private readonly WriteableBitmap _bitmap;
    private readonly DispatcherTimer _timer;

    private Sim.Geometry.Point RenderScale = new Sim.Geometry.Point(1.0, 1.0);

    private const int FPS = 60;

    public const int WIDTH = 800;
    public const int HEIGHT = 800;

    public WriteableBitmap Bitmap => _bitmap;

    public Func<Positions> GetPositions { get; init; } = () => ReadOnlyDictionary<int, Sim.Geometry.PointI>.Empty;

    public event Action AfterDraw;

    public Renderer(double dpiScale)
    {
        _bitmap = new WriteableBitmap(new PixelSize(WIDTH, HEIGHT), new Vector(96 * dpiScale, 96 * dpiScale), PixelFormat.Bgra8888);

        var interval = TimeSpan.FromSeconds(1) / FPS;
        _timer = new DispatcherTimer { Interval = interval };
        _timer.Tick += Draw;
    }

    public void Run() => _timer.Start();

    public void SetRenderScale(Sim.Geometry.Point renderScale) => RenderScale = renderScale;

    private void Draw(object sender, EventArgs e)
    {
        var positions = GetPositions();
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

        AfterDraw?.Invoke();
    }
}

