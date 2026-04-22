using Sim.Geometry;
using SkiaSharp;

namespace Sim.View;

public class PanCalculator
{
    public Point RelTopLeft { get; private set; }

    public SKPoint GetAbsTopLeft(Renderer renderer) => new SKPoint(renderer.Width * (float)RelTopLeft.X, renderer.Height * (float)RelTopLeft.Y);

    public void ApplyPan(Renderer renderer, ref SKRect p) => p.Offset(GetAbsTopLeft(renderer).Negate());
    public void ApplyPan(Renderer renderer, ref (SKPoint, SKPoint) p)
    {
        var (a, b) = p;
        a.Offset(GetAbsTopLeft(renderer).Negate());
        b.Offset(GetAbsTopLeft(renderer).Negate());
        p = (a, b);
    }

    public void Move(Point relOffset) => RelTopLeft += relOffset;

    public void Reset() => RelTopLeft = new Point(0, 0);
}
