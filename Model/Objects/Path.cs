using System.Collections.Generic;
using Sim.Geometry;

namespace Sim.Model.Objects;

internal class Path
{
    private LinkedList<Point> Points { get; } = [];

    public Point Start => Points.First.Value;
    public Point End => Points.Last.Value;

    public bool IsCovered => TargetNode is null;

    public Point TargetPoint => TargetNode.Value;
    public LinkedListNode<Point> TargetNode { get; private set; }

    public void OnTargetReached() => TargetNode = TargetNode?.Next;

    public void Populate(IReadOnlyCollection<Point> points)
    {
        New(Start, End);

        var node = Points.First;
        foreach (var p in points)
            node = Points.AddAfter(node, p);

        TargetNode = Points.First.Next;
    }

    public void New(Point start, Point end)
    {
        Points.Clear();
        Points.AddFirst(start);
        Points.AddLast(end);

        TargetNode = Points.Last;
    }
}
