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
    private const double EVADE_DISTANCE_FACTOR = 10;
    private const double STRAIGHT_LINE_THRESHOLD = 0.001;

    public static double GetEvadeDistance(Size size)
    {
        var maxDist = Math.Max(size.Width * EVADE_DISTANCE_FACTOR, size.Height * EVADE_DISTANCE_FACTOR);
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

    private void EvadeObjects(Movable movable, ref Point absTarget)
    {
        var movableRect = map[movable.Id];
        var objectRects = GetNearbyObjectRects(movableRect);
        var evadeDist = GetEvadeDistance(movableRect.Size);
        var relTarget = absTarget - movableRect.Pos;
        (double minDist, Point newTarget) = (double.MaxValue, absTarget);

        foreach (var objectRect in objectRects)
        {
            var (a, b) = Rect.GetDirectVector(movableRect, objectRect);
            var distVec = (b - a);
            var dist = distVec.Length;

            if (dist >= evadeDist)
                continue;

            if (dist >= minDist)
                continue;

            minDist = dist;

            var distX = distVec.X;
            var distY = distVec.Y;

            var isVerticalToObject = Math.Abs(distX) < Math.Abs(distY);

            newTarget = isVerticalToObject
                ? HandleVerticalFromObject(objectRect, (double)relTarget.Y, distY, evadeDist, absTarget)
                : HandleHorizontalFromObject(objectRect, (double)relTarget.X, distX, evadeDist, absTarget);
        }

        absTarget = newTarget;
    }

    private Point HandleHorizontalFromObject(Rect objectRect, double relTargetX, double distX, double evadeDist, Point target)
    {
        var hasHorizontalMovement = relTargetX != 0;
        if (!hasHorizontalMovement)
            return target;

        var objectBlocksTarget = Math.Sign(distX) == Math.Sign(relTargetX) && Math.Abs(relTargetX) > Math.Abs(distX);
        if (!objectBlocksTarget)
            return target;

        var moveNorth = target.Y < objectRect.Center.Y;
        var newX = target.X - relTargetX;
        var newY = moveNorth ? objectRect.Top - evadeDist : objectRect.Bottom + evadeDist;
        return new Point(newX, newY);
    }

    private Point HandleVerticalFromObject(Rect objectRect, double relTargetY, double distY, double evadeDist, Point target)
    {
        var hasVerticalMovement = relTargetY != 0;
        if (!hasVerticalMovement)
            return target;

        var objectBlocksTarget = Math.Sign(distY) == Math.Sign(relTargetY) && Math.Abs(relTargetY) > Math.Abs(distY);
        if (!objectBlocksTarget)
            return target;

        var moveWest = target.X < objectRect.Center.X;
        var newX = moveWest ? objectRect.Left - evadeDist : objectRect.Right + evadeDist;
        var newY = target.Y - relTargetY;
        return new Point(newX, newY);
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
