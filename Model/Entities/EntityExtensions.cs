using Sim.Geometry;

namespace Sim.Model.Entities;

internal static class RectExtensions
{
    public static PointI ToAbsPoint(this Point point, WorldSettings settings) => (point * new Point(settings.MapWidth, settings.MapHeight)).ToPointI();

    public static RectI ToAbsRect(this Rect rect, WorldSettings settings)
    {
        var dims = new Size(settings.MapWidth, settings.MapHeight);
        var pos = rect.Pos * dims.ToPoint();
        var size = rect.Size * dims;
        var absRect = new Rect(pos, size);
        return absRect.ToRectI();
    }
}
