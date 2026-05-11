using System.Collections.Generic;
using Sim.Geometry;
using Sim.Utils;

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

    public void UpdateTarget(Point currentPos)
    {
        if (CMP.Equals(currentPos, TargetPoint))
            TargetNode = TargetNode?.Next;
    }

    internal LinkedListNode<Point> AddAfter(LinkedListNode<Point> nodeToAddAfter, Point point)
    {
        var newNode = Points.AddAfter(nodeToAddAfter, point);
        if (nodeToAddAfter == StartNode)
            TargetNode = newNode;

        return newNode;
    }

    internal void Remove(LinkedListNode<Point> nodeToRemove)
    {
        if (nodeToRemove == TargetNode)
        {
            if (nodeToRemove.Next != null)
                TargetNode = nodeToRemove.Next;
            else
                TargetNode = nodeToRemove.Previous;
        }

        Points.Remove(nodeToRemove);
    }
}
