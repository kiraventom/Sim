using System.Collections.Generic;
using Sim.Geometry;

namespace Sim.Model.Objects;

public class Path
{
    private LinkedList<Point> Points { get; } = [];

    public Point Start => StartNode.Value;
    public Point End => EndNode.Value;

    public LinkedListNode<Point> StartNode => Points.First;
    public LinkedListNode<Point> EndNode => Points.Last;

    public bool IsCovered => TargetNode is null;

    public Point TargetPoint => TargetNode.Value;
    public LinkedListNode<Point> TargetNode { get; private set; }

    public Path(Point start, Point end)
    {
        Points.AddFirst(start);
        Points.AddLast(end);
        TargetNode = Points.Last;
    }

    public void OnTargetReached() => TargetNode = TargetNode?.Next;

    internal LinkedListNode<Point> AddAfter(LinkedListNode<Point> nodeToAddAfter, Point point)
    {
        var newNode = Points.AddAfter(nodeToAddAfter, point);
        if (nodeToAddAfter == StartNode)
            TargetNode = newNode;

        return newNode;
    }
}
