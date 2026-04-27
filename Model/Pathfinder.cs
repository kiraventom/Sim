using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Sim.Geometry;
using Sim.Model.Objects;

namespace Sim.Model;

internal class Pathfinder(ILogger<Pathfinder> logger, Map map)
{
    private const int LOOK_AROUND_FACTOR = 1;
    private const double PUSH_DISTANCE_FACTOR = 10;
    private const double STRAIGHT_LINE_THRESHOLD = 0.001;

    public static double GetMaxPushDistance(Size size)
    {
        var maxDist = Math.Max(size.Width * PUSH_DISTANCE_FACTOR, size.Height * PUSH_DISTANCE_FACTOR);
        return maxDist;
    }

    public RectI GetLookAroundGrid(Rect rect)
    {
        var areaSize = 1.0 / Map.AREAS_COUNT;
        var inflatedRect = new Rect(rect.Pos - new Point(areaSize, areaSize) * LOOK_AROUND_FACTOR, new Size(areaSize, areaSize) * 2 * LOOK_AROUND_FACTOR);
        var grid = map.GetAreaGrid(inflatedRect);
        return grid;
    }

    public Point GetAdjustedTarget(Movable movable, Point target)
    {
        EvadeObjects(movable, ref target);
        return target;
    }

    private void EvadeObjects(Movable movable, ref Point target)
    {
        var movableRect = map[movable.Id];
        var objectRects = GetNearbyObjectRects(movableRect);
        var maxDist = GetMaxPushDistance(movableRect.Size);
        var pureTarget = target - movableRect.Pos;
        var newTargets = new Dictionary<double, Point>();

        foreach (var objectRect in objectRects)
        {
            var (a, b) = Rect.GetDirectVector(movableRect, objectRect);
            var distVec = (b - a);
            var dist = distVec.Length;

            if (dist > maxDist)
                continue;

            var pureX = pureTarget.X;
            var pureY = pureTarget.Y;

            var distX = distVec.X;
            var distY = distVec.Y;

            var diffX = pureX - distX;
            var diffY = pureY - distY;

            Point newTarget;
            if (distX == 0 && (pureY == 0 || Math.Sign(distY) == Math.Sign(pureY)))
            {
                var sign = Math.Sign(target.X - objectRect.Center.X);

                var newX = target.X + Math.Abs(diffY) * sign;
                var newY = target.Y - diffY;
                newTarget = new Point(newX, newY);
            }
            else if (distY == 0 && (pureX == 0 || Math.Sign(distX) == Math.Sign(pureX)))
            {
                var sign = Math.Sign(target.Y - objectRect.Center.Y);

                var newX = target.X - diffX;
                var newY = target.Y + Math.Abs(diffX) * sign;
                newTarget = new Point(newX, newY);
            }
            else
            {
                newTarget = target;
            }

            logger.LogDebug("target: {oldT}, pure: {PX} {PY}, dist: {DX} {DY}, diff: {dX} {dY}, new target: {T}", target, pureX, pureY, distX, distY, diffX, diffY, newTarget);
            newTargets[dist] = newTarget;
        }

        if (newTargets.Any())
            target = newTargets.MinBy(x => x.Key).Value;
    }

    private IEnumerable<Rect> GetNearbyObjectRects(Rect movableRect)
    {
        var grid = GetLookAroundGrid(movableRect);
        var areas = map.GetAreasByGrid(grid);

        HashSet<Rect> rects = [];

        foreach (var area in areas)
        {
            foreach (var objId in area.ObjectIds)
            {
                var rect = map[objId];
                if (rect == movableRect)
                    continue;

                rects.Add(rect);
            }
        }

        return rects;
    }
}
