using Sim.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace Sim.Model;

internal class Raycaster
{
    public const double RAYCAST_STEP = 0.00001;

    private Map Map { get; }
    private HashSet<int> IgnoredIds { get; }

    private RaycastHit? CurrentHit { get; set; }
    private Point Current { get; set; }
    private RaycastResult Result { get; set; }

    public Raycaster(Map map, IReadOnlyCollection<int> ignoredIds = null)
    {
        Map = map;
        IgnoredIds = ignoredIds?.ToHashSet() ?? [];
    }

    public RaycastResult Cast(Point start, Point target)
    {
        var ray = target - start;
        var rayLength = ray.Length;

        if (rayLength == 0)
            return RaycastResult.NoHits;

        var dir = ray.Normalize();
        double dist = 0;

        Current = start;
        CurrentHit = null;
        Result = new RaycastResult();

        while (dist <= rayLength)
        {
            Current += dir * RAYCAST_STEP;
            dist += RAYCAST_STEP;

            if (!Current.IsOnMap())
                break;

            if (!TryAddExit())
                continue;

            TryAddEnter();
        }

        TryAddExit(force: true);

        return Result;
    }

    private bool TryAddExit(bool force = false)
    {
        if (CurrentHit is {} h)
        {
            if (!force && Map[h.Id].Contains(Current))
                return false;

            var hit = new RaycastHit(h.Id, h.Enter, Current);
            Result.Add(hit);
            CurrentHit = null;
        }

        return true;
    }

    private bool TryAddEnter()
    {
        var grid = Map.GetAreaGrid(new Rect(Current, new Size(RAYCAST_STEP, RAYCAST_STEP)));

        foreach (var area in Map.GetAreasByGrid(grid))
        {
            foreach (var id in area.ObjectIds)
            {
                if (IgnoredIds.Contains(id) || Result.ContainsId(id))
                    continue;

                if (Map[id].Contains(Current))
                {
                    CurrentHit = new RaycastHit(id, Current, Point.INVALID);
                    return true;
                }
            }
        }

        return false;
    }
}

