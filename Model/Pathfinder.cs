using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Sim.Geometry;
using Sim.Model.Objects;

namespace Sim.Model;

internal class Pathfinder(ILogger<Pathfinder> logger, Map map)
{
    private record AltTarget(double Distance, IReadOnlyCollection<Point> Points);
    private enum EvasionResult { NoChange, AltTarget, Fail }

    private const int LOOK_AROUND_FACTOR = 1;
    private const double EVADE_DISTANCE_FACTOR = 5;

    public static double GetEvadeDistance(Size size)
    {
        var maxDist = Math.Max(size.Width * EVADE_DISTANCE_FACTOR, size.Height * EVADE_DISTANCE_FACTOR);
        return maxDist;
    }

    public RectI GetLookAroundGrid(Rect rect)
    {
        var areaSize = 1.0 / Map.AREAS_COUNT;
        var inflatedRect = new Rect(rect.Pos, new Size(areaSize, areaSize) * 2 * LOOK_AROUND_FACTOR);
        var grid = map.GetAreaGrid(inflatedRect);
        return grid;
    }

    public void AdjustMovement(Movable movable)
    {
        if (movable.Movement.Points.Count > 1)
            return;

        var target = movable.Movement.GetTarget();
        var result = EvadeObjects(movable, target, out var altTarget);
        if (result == EvasionResult.AltTarget)
        {
            movable.Movement.Points.AddFirst(altTarget);
        }
        else if (result == EvasionResult.Fail)
        {
            // TODO: Find a way out
            // TEMP
            movable.Movement.Populate(map[movable.Id].Pos, map.RandomFreeRect(movable.Size).Pos);
        }
    }

    private EvasionResult EvadeObjects(Movable movable, Point absTarget, out Point altTarget)
    {
        altTarget = absTarget;

        var movableRect = map[movable.Id];
        var objectRects = GetNearbyObjectRects(movableRect);
        var evadeDist = GetEvadeDistance(movableRect.Size);
        var relTarget = absTarget - movableRect.Pos;
        List<AltTarget> altTargets = [];

        // TODO: Move to selecting closest object and evading only it
        foreach (var objectRect in objectRects)
        {
            var (a, b) = Rect.GetDirectVector(movableRect, objectRect);
            var distVec = (b - a);
            var dist = distVec.Length;

            if (dist >= evadeDist)
                continue;

            var distX = distVec.X;
            var distY = distVec.Y;

            var isVerticalToObject = Math.Abs(distX) < Math.Abs(distY);

            var points = isVerticalToObject
                ? GetHorizontalPoints(movableRect, objectRect, (double)relTarget.Y, distY, evadeDist, absTarget)
                : GetVerticalPoints(movableRect, objectRect, (double)relTarget.X, distX, evadeDist, absTarget);

            if (points.Count > 0)
                altTargets.Add(new AltTarget(dist, points));
        }

        if (altTargets.Count == 0)
        {
            logger.LogDebug("No objects in evade distance near {Pos}", movableRect.Pos);
            return EvasionResult.NoChange;
        }

        altTargets.Sort((a, b) => a.Distance.CompareTo(b.Distance));
        foreach (var (_, points) in altTargets)
        {
            foreach (var point in points)
            {
                var rect = new Rect(point, movable.Size);
                if (rect.Left < 0 || rect.Right > 1.0 || rect.Top < 0 || rect.Bottom > 1.0)
                    continue;

                var grid = map.GetAreaGrid(rect);
                if (map.CanPlace(grid, rect, movable.Id))
                {
                    altTarget = point;
                    return EvasionResult.AltTarget;
                }
            }
        }

        logger.LogError("Failed to calculate alt target");
        return EvasionResult.Fail;
    }

    private IReadOnlyCollection<Point> GetVerticalPoints(Rect movableRect, Rect objectRect, double relTargetX, double distX, double evadeDist, Point target)
    {
        var hasHorizontalMovement = relTargetX != 0;
        if (!hasHorizontalMovement)
            return [];

        var objectBlocksTarget = Math.Sign(distX) == Math.Sign(relTargetX) && Math.Abs(relTargetX) > Math.Abs(distX);
        if (!objectBlocksTarget)
            return [];

        var moveNorth = target.Y < objectRect.Center.Y;
        var x = movableRect.Pos.X;
        var northY = objectRect.Top - evadeDist;
        var southY = objectRect.Bottom + evadeDist;

        var north = new Point(x, northY);
        var south = new Point(x, southY);

        // If alt point is backwards
        if (movableRect.Pos.Y < objectRect.Top && movableRect.Pos.Y > northY)
        {
            if (movableRect.Pos.Y > objectRect.Bottom && movableRect.Pos.Y < southY)
                return [];

            return [south];
        }

        if (moveNorth)
            return [north, south];

        return [south, north];
    }

    private IReadOnlyCollection<Point> GetHorizontalPoints(Rect movableRect, Rect objectRect, double relTargetY, double distY, double evadeDist, Point target)
    {
        var hasVerticalMovement = relTargetY != 0;
        if (!hasVerticalMovement)
            return [];

        var objectBlocksTarget = Math.Sign(distY) == Math.Sign(relTargetY) && Math.Abs(relTargetY) > Math.Abs(distY);
        if (!objectBlocksTarget)
            return [];

        var moveWest = target.X < objectRect.Center.X;
        var westX = objectRect.Left - evadeDist;
        var eastX = objectRect.Right + evadeDist;
        var y = movableRect.Pos.Y;

        var west = new Point(westX, y);
        var east = new Point(eastX, y);

        // If alt point is backwards
        if (movableRect.Pos.X < objectRect.Left && movableRect.Pos.X > westX)
        {
            if (movableRect.Pos.X > objectRect.Right && movableRect.Pos.X < eastX)
                return [];

            return [east];
        }

        if (moveWest)
            return [west, east];

        return [east, west];    
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
