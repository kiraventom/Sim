using Microsoft.Extensions.Logging;
using Sim.Geometry;
using Sim.Model.Objects;
using System.Collections.Generic;

namespace Sim.Model;

internal class Raycaster
{
    public const double STEP = 0.00001;

    private ILogger<Raycaster> Logger { get; }
    private World World { get; }
    private Map Map { get; }
    private int Id { get; }
    private Size Size { get; }

    private Rect CurrentRect { get; set; }

    private RectI? _grid;
    private IEnumerable<Area> _areas;

    public Raycaster(ILogger<Raycaster> logger, World world, Map map, int movableId)
    {
        Logger = logger;
        World = world;
        Map = map;
        Id = movableId;
        Size = map[Id].Size;
    }

    public RaycastResult Cast(Point start, Point target, bool ignoreMovables = true)
    {
        var ray = target - start;
        var rayLength = ray.Length;

        if (rayLength == 0)
            return RaycastResult.NO_HITS;

        var dir = ray.Normalize();
        double dist = 0;

        CurrentRect = new Rect(start, Size);

        while (dist <= rayLength)
        {
            CurrentRect = CurrentRect.Offset(dir * STEP);
            dist += STEP;

            if (!CurrentRect.Pos.IsOnMap())
                break;

            if (RegisterHit(ignoreMovables, out var hit))
                return new RaycastResult(hit);
        }

        return RaycastResult.NO_HITS;
    }

    private bool RegisterHit(bool ignoreMovables, out RaycastHit hit)
    {
        var grid = Map.GetAreaGrid(CurrentRect);
        if (grid != _grid)
        {
            _grid = grid;
            _areas = Map.GetAreasByGrid(grid);
        }

        foreach (var area in _areas)
        {
            foreach (var id in area.ObjectIds)
            {
                if (id == Id)
                    continue;

                if (ignoreMovables && World.Objects[id] is Movable)
                    continue;

                var objectRect = Map[id];
                if (objectRect.Intersects(CurrentRect))
                {
                    var intersectionPoint = GetIntersectionPoint(objectRect, CurrentRect);
                    if (intersectionPoint.IsInvalid())
                    {
                        Logger.LogWarning("Failed to get intersection point for object {Obj}, movable {Mov}", objectRect, CurrentRect);
                        continue;
                    }

                    hit = new RaycastHit(id, intersectionPoint);
                    return true;
                }
            }
        }

        hit = RaycastHit.INVALID;
        return false;
    }

    private Point GetIntersectionPoint(Rect objectRect, Rect movableRect)
    {
        var topLeftHit = objectRect.Contains(movableRect.TopLeft);
        var topRightHit = objectRect.Contains(movableRect.TopRight);
        var bottomRightHit = objectRect.Contains(movableRect.BottomRight);
        var bottomLeftHit = objectRect.Contains(movableRect.BottomLeft);

        var topHit = topLeftHit && topRightHit;
        var bottomHit = bottomLeftHit && bottomRightHit;
        var leftHit = topLeftHit && bottomLeftHit;
        var rightHit = topRightHit && bottomRightHit;

        if (topHit)
            return new Point(movableRect.Center.X, objectRect.Bottom);

        if (bottomHit)
            return new Point(movableRect.Center.X, objectRect.Top);

        if (leftHit)
            return new Point(objectRect.Right, movableRect.Center.Y);

        if (rightHit)
            return new Point(objectRect.Left, movableRect.Center.Y);

        if (topLeftHit)
            return objectRect.BottomRight;

        if (topRightHit)
            return objectRect.BottomLeft;

        if (bottomLeftHit)
            return objectRect.TopRight;

        if (bottomRightHit)
            return objectRect.TopLeft;

        return Point.INVALID;
    }
}

