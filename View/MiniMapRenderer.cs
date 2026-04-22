using Sim.Host;
using SkiaSharp;

namespace Sim.View;

public class MiniMapRenderer : Renderer
{
    protected override int FPS => 15;

    public MiniMapRenderer(int width, int height, double dpiScale, ZoomCalculator zoomCalc, PanCalculator panCalc) : base(width, height, dpiScale, zoomCalc, panCalc)
    {
    }

    protected override void DrawInternal(SKCanvas canvas)
    {
        var size = ZoomCalc.GetSize(this);
        var topLeft = PanCalc.GetAbsTopLeft(this);

        var left = (float)(topLeft.X / ZoomCalc.Zoom);
        var top = (float)(topLeft.Y / ZoomCalc.Zoom);
        var right = (float)(left + size.Width) - 1;
        var bottom = (float)(top + size.Height) - 1;

        var rect = new SKRect(left, top, right, bottom);
        DrawRect(canvas, ref rect, Brushes.Visor);
    }

    protected override void ApplyZoomPan(ref SKRect rect) { }
    protected override void ApplyZoomPan(ref (SKPoint, SKPoint) rect) { }
    protected override void DrawInfo(SKCanvas canvas, SKRect rect, IHumanEntity humanEntity) {}
}
