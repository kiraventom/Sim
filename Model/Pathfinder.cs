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

    private void EvadeObjects(Movable movable, ref Point target)
    {
        var movableRect = map[movable.Id];
        var objectRects = GetNearbyObjectRects(movableRect);
        var evadeDist = GetEvadeDistance(movableRect.Size);
        var pureTarget = target - movableRect.Pos;
        var newTargets = new Dictionary<double, Point>();

        foreach (var objectRect in objectRects)
        {
            var (a, b) = Rect.GetDirectVector(movableRect, objectRect);
            var distVec = (b - a);
            var dist = distVec.Length;

            if (dist > evadeDist)
                continue;

            var pureX = pureTarget.X;
            var pureY = pureTarget.Y;

            var distX = distVec.X;
            var distY = distVec.Y;

            Point newTarget = target;

            var isVerticalToObject = Math.Abs(distX) < Math.Abs(distY);

            if (isVerticalToObject)
            {
                HandleVerticalFromObject(objectRect, pureY, distY, evadeDist, ref newTarget);
            }
            else
            {
                HandleHorizontalFromObject(objectRect, pureX, distX, evadeDist, ref newTarget);
            }

            newTargets[dist] = newTarget;
        }

        if (newTargets.Any())
            target = newTargets.MinBy(x => x.Key).Value;
    }

    private bool HandleHorizontalFromObject(Rect objectRect, double pureX, double distX, double evadeDist, ref Point target)
    {
        var hasHorizontalMovement = pureX != 0;
        if (!hasHorizontalMovement)
            return false;

        var objectBlocksTarget = Math.Sign(distX) == Math.Sign(pureX) && Math.Abs(pureX) > Math.Abs(distX);
        if (!objectBlocksTarget)
            return false;

        var moveNorth = target.Y < objectRect.Center.Y;
        var newX = target.X - pureX;
        var newY = moveNorth ? objectRect.Top - evadeDist : objectRect.Bottom + evadeDist;
        target = new Point(newX, newY);
        return true;
    }

    private bool HandleVerticalFromObject(Rect objectRect, double pureY, double distY, double evadeDist, ref Point target)
    {
        var hasVerticalMovement = pureY != 0;
        if (!hasVerticalMovement)
            return false;

        var objectBlocksTarget = Math.Sign(distY) == Math.Sign(pureY) && Math.Abs(pureY) > Math.Abs(distY);
        if (!objectBlocksTarget)
            return false;

        var moveWest = target.X < objectRect.Center.X;
        var newX = moveWest ? objectRect.Left - evadeDist : objectRect.Right + evadeDist;
        var newY = target.Y - pureY;
        target = new Point(newX, newY);
        return true;
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
