using Sim.Geometry;

namespace Sim.Model.Objects;

internal class Movement(Point start, Point end)
{
    public Point Start => start;
    public Point End => end;

    public Point GetTarget() => End;
}
