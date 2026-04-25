using Avalonia.Platform;
using Sim.Geometry;
using Sim.Model.Entities;
using SkiaSharp;
using System;

namespace Sim.View;

public abstract class Renderer
{
    protected virtual int FPS => 60;

    private readonly Avalonia.Media.Imaging.WriteableBitmap _bitmap;
    private readonly Avalonia.Threading.DispatcherTimer _timer;

    public ZoomCalculator ZoomCalc { get; }
    public PanCalculator PanCalc { get; }

    public Func<EntitySnapshot> GetSnapshot { get; init; } = () => new EntitySnapshot();
    public Func<int> GetSelectedObjectId { get; init; } = () => -1;

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
        var snapshot = GetSnapshot();
        if (snapshot is null)
            return;

        var selectedObjectId = GetSelectedObjectId();

        using var buf = _bitmap.Lock();
        var info = new SKImageInfo(Width, Height, SKColorType.Bgra8888);
        using var surface = SKSurface.Create(info, buf.Address, buf.RowBytes);

        var canvas = surface.Canvas;

        canvas.Clear(SKColors.Black);

        DrawBackground(canvas);
        DrawEntities(snapshot, selectedObjectId, canvas);

        DrawInternal(canvas);

        AfterDraw?.Invoke();
    }

    private void DrawBackground(SKCanvas canvas)
    {
        var rect = new SKRect(0, 0, Width, Height);
        DrawRect(canvas, ref rect, Brushes.Background);
    }

    private void DrawEntities(EntitySnapshot snapshot, int selectedObjectId, SKCanvas canvas)
    {
        foreach (var entity in snapshot.Obstacles)
        {
            var isSelected = entity.ObjectId == selectedObjectId;
            var brush = isSelected ? Brushes.SelectedObstacle : Brushes.Obstacle;
            
            var rect = ToSKRect(entity.Rect);
            ApplyRenderScale(ref rect);
            DrawRect(canvas, ref rect, brush);

            if (isSelected)
                DrawInfo(canvas, rect, entity.ObjectId);
        }

        foreach (var entity in snapshot.Lines)
        {
            var isSelected = entity.ObjectId == selectedObjectId;
            var brush = isSelected ? Brushes.SelectedLine : Brushes.Line;

            var points = ToSKPoints(entity.A, entity.B);
            ApplyRenderScale(ref points);
            DrawLine(canvas, points, brush);
        }

        foreach (var entity in snapshot.Areas)
        {
            var isSelected = entity.ObjectId == selectedObjectId;
            if (!isSelected)
                continue;

            var brush = Brushes.Area;
            
            var rect = ToSKRect(entity.Rect);
            ApplyRenderScale(ref rect);
            DrawRect(canvas, ref rect, brush);
        }

        foreach (var entity in snapshot.Humans)
        {
            var isSelected = entity.ObjectId == selectedObjectId;
            var brush = isSelected ? Brushes.SelectedHuman : Brushes.Human;
            
            var rect = ToSKRect(entity.Rect);
            ApplyRenderScale(ref rect);
            DrawRect(canvas, ref rect, brush);

            if (isSelected)
                DrawInfo(canvas, rect, entity.ObjectId);
        }
    }

    protected virtual void DrawInfo(SKCanvas canvas, SKRect rect, int objectId)
    {
        var text = SKTextBlob.Create(objectId.ToString(), Fonts.Info);
        var textPoint = new SKPoint(rect.Left - text.Bounds.MidX, rect.Top - text.Bounds.Height / 2);

        canvas.DrawText(text, textPoint.X, textPoint.Y, Brushes.Info);
    }

    protected void DrawRect(SKCanvas canvas, ref SKRect rect, SKPaint brush)
    {
        ApplyZoomPan(ref rect);

        if (rect.Right < 0 || rect.Left > Width || rect.Bottom < 0 || rect.Top > Height)
            return;

        ScaleSmallRect(ref rect, ref brush);
        canvas.DrawRect(rect, brush);
    }

    protected void DrawLine(SKCanvas canvas, (SKPoint, SKPoint) points, SKPaint brush)
    {
        ApplyZoomPan(ref points);

        var (a, b) = points;
        canvas.DrawLine(a, b, brush);
    }

    private static void ScaleSmallRect(ref SKRect rect, ref SKPaint brush)
    {
        float wRatio = rect.Width < 1 ? rect.Width : 1;
        float hRatio = rect.Height < 1 ? rect.Height : 1;
        if (wRatio >= 1 && hRatio >= 1)
            return;

        var minRatio = Math.Min(wRatio, hRatio);
        rect.Inflate(wRatio < 1 ? (1 - rect.Width) / 2f : 0, hRatio < 1 ? (1 - rect.Height) / 2f : 0);

        var alpha = (byte)Math.Round(minRatio * 255);
        if (alpha != brush.Color.Alpha)
            brush = Brushes.ChangeColor(brush, brush.Color.WithAlpha(alpha));
    }

    private SKRect ToSKRect(RectI rect)
    {
        return new SKRect(rect.Left, rect.Top, rect.Right, rect.Bottom);
    }

    private (SKPoint, SKPoint) ToSKPoints(PointI pA, PointI pB)
    {
        var a = new SKPoint(pA.X, pA.Y);
        var b = new SKPoint(pB.X, pB.Y);
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
