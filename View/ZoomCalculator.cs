using Sim.Geometry;
using SkiaSharp;
using System;

namespace Sim.View;

public class ZoomCalculator
{
    private const float FACTOR = 1.2f;
    private const float MIN_ZOOM = 0.5f;

    public float Zoom { get; private set; } = 1.0f;
    
    public Size GetSize(Renderer renderer) => new Size(renderer.Width / Zoom, renderer.Height / Zoom);

    public void ApplyZoom(ref SKRect p)
    {
        p = new SKRect(p.Left * Zoom, p.Top * Zoom, p.Right * Zoom, p.Bottom * Zoom);
    }

    public void ApplyZoom(ref (SKPoint, SKPoint) p)
    {
        var (a, b) = p;
        p = (new SKPoint(a.X * Zoom, a.Y * Zoom), new SKPoint(b.X * Zoom, b.Y * Zoom));
    }

    public void ZoomIn(PanCalculator panCalc)
    {
        var oldZoom = Zoom;
        Zoom *= FACTOR;
        AdjustPan(panCalc, oldZoom, Zoom);
    }

    public void ZoomOut(PanCalculator panCalc)
    {
        var oldZoom = Zoom;
        Zoom = Math.Max(MIN_ZOOM, Zoom / FACTOR);
        AdjustPan(panCalc, oldZoom, Zoom);
    }

    private void AdjustPan(PanCalculator panCalc, float oldZoom, float newZoom)
    {
        if (oldZoom == newZoom) return;
        var ratio = newZoom / oldZoom;
        var dx = (0.5 + panCalc.RelTopLeft.X) * (ratio - 1);
        var dy = (0.5 + panCalc.RelTopLeft.Y) * (ratio - 1);
        panCalc.Move(new Point(dx, dy));
    }

    public void Reset() => Zoom = 1.0f;
}


