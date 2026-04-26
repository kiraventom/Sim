using System.Collections.Generic;
using Sim.Geometry;

namespace Sim.Model.Objects;

internal class Path
{
    private LinkedList<Point> Points { get; } = [];

    public Point Start => StartNode.Value;
    public Point End => EndNode.Value;

    public LinkedListNode<Point> StartNode => Points.First;
    public LinkedListNode<Point> EndNode => Points.Last;

    public bool IsCovered => TargetNode is null;

    public Point TargetPoint => TargetNode.Value;
    public LinkedListNode<Point> TargetNode { get; private set; }

    public void OnTargetReached() => TargetNode = TargetNode?.Next;

    public LinkedListNode<Point> AddAfter(LinkedListNode<Point> nodeToAddAfter, Point point)
    {
        var newNode = Points.AddAfter(nodeToAddAfter, point);
        if (nodeToAddAfter == StartNode)
            TargetNode = newNode;

        return newNode;
    }

    public void New(Point start, Point end)
    {
        Points.Clear();
        Points.AddFirst(start);
        Points.AddLast(end);

        TargetNode = Points.Last;
    }
}
