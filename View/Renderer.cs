using Avalonia.Platform;
using Sim.Geometry;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;
using Positions = System.Collections.Generic.IReadOnlyDictionary<int, Sim.Geometry.PointI>;

namespace Sim.View;

public class Renderer
{
    private const int FPS = 60;
    public const int WIDTH = 800;
    public const int HEIGHT = 800;

    private readonly Avalonia.Media.Imaging.WriteableBitmap _bitmap;
    private readonly Avalonia.Threading.DispatcherTimer _timer;

    private ZoomCalculator ZoomCalc { get; }
    private PanCalculator PanCalc { get; }

    private Point RenderScale = new Point(1.0, 1.0);

    public Avalonia.Media.Imaging.WriteableBitmap Bitmap => _bitmap;
    public Func<Positions> GetPositions { get; init; } = () => ReadOnlyDictionary<int, PointI>.Empty;
    public event Action AfterDraw;

    public Renderer(double dpiScale, ZoomCalculator zoomCalc, PanCalculator panCalc)
    {
        _bitmap = new Avalonia.Media.Imaging.WriteableBitmap(new Avalonia.PixelSize(WIDTH, HEIGHT), new Avalonia.Vector(96 * dpiScale, 96 * dpiScale), PixelFormat.Bgra8888);

        ZoomCalc = zoomCalc;
        PanCalc = panCalc;

        var interval = TimeSpan.FromSeconds(1) / FPS;
        _timer = new Avalonia.Threading.DispatcherTimer { Interval = interval };
        _timer.Tick += Draw;
    }

    public void Run() => _timer.Start();

    public void SetRenderScale(Point renderScale) => RenderScale = renderScale;

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

        DrawBackground(canvas);
        DrawHumans(positions, canvas);

        AfterDraw?.Invoke();
    }

    private void DrawBackground(SKCanvas canvas)
    {
        var rect = new SKRect(0, 0, WIDTH, HEIGHT);

        ApplyRenderScale(ref rect);
        ZoomCalc.ApplyZoom(ref rect);
        PanCalc.ApplyPan(ref rect);

        canvas.DrawRect(rect, Brushes.Empty);
    }

    private void DrawHumans(Positions positions, SKCanvas canvas)
    {
        foreach (var pos in positions)
        {
            var rect = ToSKRect(pos.Value);

            ApplyRenderScale(ref rect);
            ZoomCalc.ApplyZoom(ref rect);
            PanCalc.ApplyPan(ref rect);

            if (rect.Right < 0 || rect.Left > WIDTH || rect.Bottom < 0 || rect.Top > HEIGHT)
                continue;

            canvas.DrawRect(rect, Brushes.Human);
        }
    }

    private SKRect ToSKRect(PointI point)
    {
        var left = (float)point.X;
        var top = (float)point.Y;
        var right = left + 1;
        var bottom = top + 1;
        return new SKRect(left, top, right, bottom);
    }

    private void ApplyRenderScale(ref SKRect rect)
    {
        rect = new SKRect(rect.Left * (float)RenderScale.X, rect.Top * (float)RenderScale.Y, rect.Right * (float)RenderScale.X, rect.Bottom * (float)RenderScale.Y);
    }
}

