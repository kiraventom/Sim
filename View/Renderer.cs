using Avalonia.Platform;
using Sim.Geometry;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;
using Positions = System.Collections.Generic.IReadOnlyDictionary<int, Sim.Geometry.PointI>;

namespace Sim.View;

public abstract class Renderer
{
    protected virtual int FPS => 60;

    private readonly Avalonia.Media.Imaging.WriteableBitmap _bitmap;
    private readonly Avalonia.Threading.DispatcherTimer _timer;

    public ZoomCalculator ZoomCalc { get; }
    public PanCalculator PanCalc { get; }

    public Func<Positions> GetPositions { get; init; } = () => ReadOnlyDictionary<int, PointI>.Empty;

    protected Point RenderScale = new Point(1.0, 1.0);

    public int Width { get; }
    public int Height { get; }

    public Avalonia.Media.Imaging.WriteableBitmap Bitmap => _bitmap;
    public event Action AfterDraw;

    public Renderer(int width, int height, double dpiScale, ZoomCalculator zoomCalc, PanCalculator panCalc)
    {
        _bitmap = new Avalonia.Media.Imaging.WriteableBitmap(new Avalonia.PixelSize(width, height), new Avalonia.Vector(96 * dpiScale, 96 * dpiScale), PixelFormat.Bgra8888);
        Width = width;
        Height = height;

        ZoomCalc = zoomCalc;
        PanCalc = panCalc;

        var interval = TimeSpan.FromSeconds(1) / FPS;
        _timer = new Avalonia.Threading.DispatcherTimer { Interval = interval };
        _timer.Tick += Draw;
    }

    public void Run() => _timer.Start();

    public void SetRenderScale(Point renderScale) => RenderScale = renderScale;

    protected virtual void DrawInternal(SKCanvas canvas)
    {
    }

    protected virtual void ApplyZoomPan(ref SKRect rect)
    {
        ZoomCalc.ApplyZoom(ref rect);
        PanCalc.ApplyPan(this, ref rect);
    }

    private void Draw(object sender, EventArgs e)
    {
        var positions = GetPositions();
        if (positions is null)
            return;

        using var buf = _bitmap.Lock();
        var info = new SKImageInfo(Width, Height, SKColorType.Bgra8888);
        using var surface = SKSurface.Create(info, buf.Address, buf.RowBytes);

        var canvas = surface.Canvas;

        canvas.Clear(SKColors.Black);

        DrawBackground(canvas);
        DrawHumans(positions, canvas);

        DrawInternal(canvas);

        AfterDraw?.Invoke();
    }

    private void DrawBackground(SKCanvas canvas)
    {
        var rect = new SKRect(0, 0, Width, Height);
        DrawRect(canvas, rect, Brushes.Background);
    }

    private void DrawHumans(Positions positions, SKCanvas canvas)
    {
        foreach (var pos in positions)
        {
            var rect = ToSKRect(pos.Value);
            ApplyRenderScale(ref rect);
            DrawRect(canvas, rect, Brushes.Human);
        }
    }

    protected void DrawRect(SKCanvas canvas, SKRect rect, SKPaint brush)
    {
        ApplyZoomPan(ref rect);

        if (rect.Right < 0 || rect.Left > Width || rect.Bottom < 0 || rect.Top > Height)
            return;

        byte alpha = 255;
        
        if (rect.Width < 1)
        {
            rect.Inflate((float)(1 - rect.Width), 0);
            alpha = (byte)Math.Round(rect.Width * 255);
        }

        if (rect.Height < 1)
        {
            rect.Inflate(0, 1 - rect.Height);
            alpha = (byte)Math.Round(rect.Height * 255);
        }

        brush.Color = brush.Color.WithAlpha(alpha);
        canvas.DrawRect(rect, brush);

        brush.Dispose();
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

