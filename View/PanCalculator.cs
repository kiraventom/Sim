using Sim.Geometry;
using SkiaSharp;

namespace Sim.View;

public class PanCalculator
{
    private Point TopLeft;

    public SKPoint GetTopLeft(Renderer renderer) => new SKPoint(renderer.Width * (float)TopLeft.X, renderer.Height * (float)TopLeft.Y);

    public void ApplyPan(Renderer renderer, ref SKRect p) => p.Offset(GetTopLeft(renderer).Negate());

    public void Move(Point offset) => TopLeft += offset;

    public void Reset() => TopLeft = new Point(0, 0);
}

public static class SKPointExtensions
{
    public static SKPoint Negate(this SKPoint point) => new SKPoint(-point.X, -point.Y);
}
