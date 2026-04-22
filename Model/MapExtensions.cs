using Sim.Geometry;
using Sim.Utils;

namespace Sim.Model;

internal static class MapExtensions
{
    public static Point RandomFreePos(this Map map) => RND.Point(1, 1);
}

