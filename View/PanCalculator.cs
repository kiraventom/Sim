using SkiaSharp;

namespace Sim.View;

public class PanCalculator
{
    public SKPoint TopLeft { get; private set; }

    public void ApplyPan(ref SKRect p) => p.Offset(-TopLeft.X, -TopLeft.Y);

    public void Move(SKPoint offset) => TopLeft += offset;

    public void Reset() => TopLeft = new SKPoint(0, 0);
}


