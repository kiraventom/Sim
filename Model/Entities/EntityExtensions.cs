using Sim.Geometry;

namespace Sim.Model.Entities;

internal static class RectExtensions
{
    public static PointI ToEntityPoint(this Point point, WorldSettings settings) => (point * new Point(settings.MapWidth, settings.MapHeight)).ToPointI();

    public static RectI ToEntityRect(this Rect rect, WorldSettings settings)
    {
        var dims = new Size(settings.MapWidth, settings.MapHeight);
        var pos = rect.TopLeft * dims.ToPoint();
        var size = rect.Size * dims;
        return new RectI(pos.ToPointI(), size.ToSizeI());
    }
}
