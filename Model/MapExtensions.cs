using Sim.Geometry;
using Sim.Utils;

namespace Sim.Model;

internal static class MapExtensions
{
    public static PointI RandomFreePos(this Map map)
    {
        PointI point;
        do
        {
            point = RND.PointI(map.Size);
        }
        while (map[point] != SpecialCell.EMPTY);

        return point;
    }
}

