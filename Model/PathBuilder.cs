using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Sim.Geometry;
using Sim.Model.Objects;
using Sim.Utils;

namespace Sim.Model;

internal class PathBuilder(ILogger<PathBuilder> logger, Map map, Raycaster raycaster, int movableId, Size movableSize)
{
    private const double EVADE_DISTANCE_MODIFIER = 2.0;
    private Size EvadeDist { get; } = movableSize * EVADE_DISTANCE_MODIFIER;

    public Path BuildPath(Point from, Point to)
    {
        var path = new Path(from, to);
        SplitLine(path, path.StartNode, path.EndNode);
        return path;
    }

    private void SplitLine(Path path, LinkedListNode<Point> start, LinkedListNode<Point> end)
    {
        var result = raycaster.Cast(start.Value, end.Value);
        if (!result.HasHit())
            return;

        var objRect = map[result.Hit.Id];
        var evadePoint = GetEvadePoint(objRect, result.Hit);

        Point point;

        if (start.Value != evadePoint.Main && CheckPoint(evadePoint.Main))
            point = evadePoint.Main;
        else if (CheckPoint(evadePoint.Alt))
            point = evadePoint.Alt;
        else
            point = Point.INVALID;

        if (point.IsInvalid())
        {
            logger.LogError("Generated target point {Target} is not accessible by {Id}, path is probably broken!", point, movableId);
            return;
        }

        var newNode = path.AddAfter(start, point);
        SplitLine(path, start, newNode);
        SplitLine(path, newNode, end);
    }

    private bool CheckPoint(Point point)
    {
        var targetRect = new Rect(point, movableSize);
        Rect.ClampToMap(ref targetRect);

        var grid = map.GetAreaGrid(targetRect);
        return map.CanPlace(grid, targetRect);
    }

    private EvadePoint GetEvadePoint(Rect objectRect, RaycastHit hit)
    {
        var leftOffset = Math.Abs(hit.Enter.X - objectRect.Left);
        var rightOffset = Math.Abs(hit.Enter.X - objectRect.Right);
        var leftHit = CMP.Equals(leftOffset, 0, Raycaster.STEP);
        var rightHit = CMP.Equals(rightOffset, 0, Raycaster.STEP);
        var leftEvade = objectRect.Left - EvadeDist.Width;
        var rightEvade = objectRect.Right + EvadeDist.Width;

        var topOffset = Math.Abs(hit.Enter.Y - objectRect.Top);
        var bottomOffset = Math.Abs(hit.Enter.Y - objectRect.Bottom);
        var topHit = CMP.Equals(topOffset, 0, Raycaster.STEP);
        var bottomHit = CMP.Equals(bottomOffset, 0, Raycaster.STEP);
        var topEvade = objectRect.Top - EvadeDist.Height;
        var bottomEvade = objectRect.Bottom + EvadeDist.Height;

        if (leftHit || rightHit)
        {
            var x = leftHit ? leftEvade : rightEvade;
            var (mainY, altY) = topOffset < bottomOffset ? (topEvade, bottomEvade) : (bottomEvade, topEvade);
            return new EvadePoint(new Point(x, mainY), new Point(x, altY));
        }

        if (topHit || bottomHit)
        {
            var (mainX, altX) = leftOffset < rightOffset ? (leftEvade, rightEvade) : (rightEvade, leftEvade);
            var y = topHit ? topEvade : bottomEvade;
            return new EvadePoint(new Point(mainX, y), new Point(altX, y));
        }

        return new EvadePoint(Point.INVALID, Point.INVALID);
    }

    private struct EvadePoint(Point main, Point alt)
    {
        public Point Main => main;
        public Point Alt => alt;
    }
}
