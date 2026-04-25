using System.Collections.Generic;
using Sim.Geometry;

namespace Sim.Model.Objects;

internal class Movement(Point start, Point end)
{
    public Point Start => start;
    public Point End => end;

    public Point GetTarget() => Points[0];

    internal List<Point> Points { get; } = [ start, end ];
}
