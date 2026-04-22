using Avalonia.Platform;
using Sim.Geometry;
using Sim.Host;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace Sim.View;

public abstract class Renderer
{
    protected virtual int FPS => 60;

    private readonly Avalonia.Media.Imaging.WriteableBitmap _bitmap;
    private readonly Avalonia.Threading.DispatcherTimer _timer;

    public ZoomCalculator ZoomCalc { get; }
    public PanCalculator PanCalc { get; }

    public Func<IReadOnlyCollection<IEntity>> GetEntities { get; init; } = () => Array.Empty<IEntity>();

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

    protected virtual void ApplyZoomPan(ref (SKPoint, SKPoint) points)
    {
        ZoomCalc.ApplyZoom(ref points);
        PanCalc.ApplyPan(this, ref points);
    }

    private void Draw(object sender, EventArgs e)
    {
        var entities = GetEntities();
        if (entities is null || entities.Count == 0)
            return;

        using var buf = _bitmap.Lock();
        var info = new SKImageInfo(Width, Height, SKColorType.Bgra8888);
        using var surface = SKSurface.Create(info, buf.Address, buf.RowBytes);

        var canvas = surface.Canvas;

        canvas.Clear(SKColors.Black);

        DrawBackground(canvas);
        DrawEntities(entities, canvas);

        DrawInternal(canvas);

        AfterDraw?.Invoke();
    }

    private void DrawBackground(SKCanvas canvas)
    {
        var rect = new SKRect(0, 0, Width, Height);
        DrawRect(canvas, rect, Brushes.Background);
    }

    private void DrawEntities(IReadOnlyCollection<IEntity> entities, SKCanvas canvas)
    {
        foreach (var entity in entities)
        {
            var brush = Brushes.GetBrush(entity);
            switch (entity)
            {
                case IRectEntity rectEntity:
                    var rect = ToSKRect(rectEntity);
                    ApplyRenderScale(ref rect);
                    DrawRect(canvas, rect, brush);
                    break;

                case ILineEntity lineEntity:
                    var points = ToSKPoints(lineEntity);
                    ApplyRenderScale(ref points);
                    DrawLine(canvas, points, brush);
                    break;
            }
        }
    }

    protected void DrawRect(SKCanvas canvas, SKRect rect, SKPaint brush)
    {
        ApplyZoomPan(ref rect);

        if (rect.Right < 0 || rect.Left > Width || rect.Bottom < 0 || rect.Top > Height)
            return;

        ScaleSmallRect(ref rect, brush);
        canvas.DrawRect(rect, brush);

        brush.Dispose();
    }

    protected void DrawLine(SKCanvas canvas, (SKPoint, SKPoint) points, SKPaint brush)
    {
        ApplyZoomPan(ref points);

        var (a, b) = points;
        canvas.DrawLine(a, b, brush);

        brush.Dispose();
    }

    private static void ScaleSmallRect(ref SKRect rect, SKPaint brush)
    {
        float wRatio = rect.Width < 1 ? rect.Width : 1;
        float hRatio = rect.Height < 1 ? rect.Height : 1;
        if (wRatio >= 1 && hRatio >= 1)
            return;

        var minRatio = Math.Min(wRatio, hRatio);
        rect.Inflate(wRatio < 1 ? (1 - rect.Width) / 2f : 0, hRatio < 1 ? (1 - rect.Height) / 2f : 0);

        var alpha = (byte)Math.Round(minRatio * 255);
        brush.Color = brush.Color.WithAlpha(alpha);
    }

    private SKRect ToSKRect(IRectEntity entity)
    {
        var left = (float)entity.Pos.X;
        var top = (float)entity.Pos.Y;
        var right = left + entity.Size.Width;
        var bottom = top + entity.Size.Height;
        return new SKRect(left, top, right, bottom);
    }

    private (SKPoint, SKPoint) ToSKPoints(ILineEntity entity)
    {
        var a = new SKPoint(entity.A.X, entity.A.Y);
        var b = new SKPoint(entity.B.X, entity.B.Y);
        return (a, b);
    }

    private void ApplyRenderScale(ref SKRect rect)
    {
        rect = new SKRect(rect.Left * (float)RenderScale.X, rect.Top * (float)RenderScale.Y, rect.Right * (float)RenderScale.X, rect.Bottom * (float)RenderScale.Y);
    }

    private void ApplyRenderScale(ref (SKPoint, SKPoint) points)
    {
        var (a, b) = points;
        points = (new SKPoint(a.X * (float)RenderScale.X, a.Y * (float)RenderScale.Y), new SKPoint(b.X * (float)RenderScale.X, b.Y * (float)RenderScale.Y));
    }
}

