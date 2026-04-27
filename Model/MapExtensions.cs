using Sim.Geometry;
using Sim.Utils;

namespace Sim.Model;

internal static class MapExtensions
{
    public static Rect RandomFreeRect(this Map map, Size size) 
    {
        for (int i = 0; i < 100; ++i)
        {
            var pos = RND.Point(1.0 - size.Width, 1.0 - size.Height);
            pos += new Point(size.Width / 2, size.Height / 2);
            var rect = new Rect(pos, size);
            var grid = map.GetAreaGrid(rect);
            if (map.CanPlace(grid, rect))
                return rect;
        }

        return Rect.INVALID;
    }
}

