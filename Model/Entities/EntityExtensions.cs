using Sim.Geometry;
using Sim.Host;
using Sim.Model.Objects;

namespace Sim.Model.Entities;

internal static class PointExtensions
{
    public static PointI ToAbsPoint(this Point point, WorldSettings settings) => (point * new Point(settings.MapWidth, settings.MapHeight)).ToPointI();
}
