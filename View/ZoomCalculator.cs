using Sim.Geometry;
using SkiaSharp;
using System;

namespace Sim.View;

public class ZoomCalculator
{
    private const float STEP = 1.0f;
    public float Zoom { get; private set; } = 1.0f;
    
    public Size GetSize(Renderer renderer) => new Size(renderer.Width / Zoom, renderer.Height / Zoom);

    public void ApplyZoom(ref SKRect p)
    {
        p = new SKRect(p.Left * Zoom, p.Top * Zoom, p.Right * Zoom, p.Bottom * Zoom);
    }

    public void ZoomIn()
    {
        Zoom += STEP;
    }

    public void ZoomOut()
    {
        Zoom = Math.Max(1.0f, Zoom - STEP);
    }

    public void Reset() => Zoom = 1.0f;
}


