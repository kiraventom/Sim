using Sim.Geometry;
using Sim.Utils;

namespace Sim.Model.Objects;

internal class Human : Movable
{
    private Map Map { get; }

    public override Size Size => new Size(0.005, 0.005);

    public Human(Map map, PathBuilder pathfinder, int id) : base(pathfinder, id)
    {
        const double SpeedModMin = 0.0015;
        const double SpeedModMax = 0.003;
        Speed = RND.Double(SpeedModMin, SpeedModMax);

        Map = map;
    }

    protected override Point GetNewTarget(Point pos)
    {
        // TEMP
        return Map.RandomFreeRect(Size).Pos;
    }
}
