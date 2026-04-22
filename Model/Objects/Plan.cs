using Sim.Geometry;

namespace Sim.Model.Objects;

internal class Plan(Point start, Point target)
{
    public Point Start => start;
    public Point Target => target;
}

