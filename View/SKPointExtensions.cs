using SkiaSharp;

namespace Sim.View;

public static class SKPointExtensions
{
    public static SKPoint Negate(this SKPoint point) => new SKPoint(-point.X, -point.Y);
}

