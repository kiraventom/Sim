using System.Collections.Generic;
using Sim.Geometry;

namespace Sim.Model.Objects;

internal class Movement
{
    public Point Start { get; private set; }
    public Point End { get; private set; }

    public Point GetTarget() => Points.First.Value;

    public LinkedList<Point> Points { get; } = [];

    public void Populate(Point start, Point end)
    {
        Start = start;
        End = end;
        
        Points.Clear();
        Points.AddFirst(end);
    }
}
