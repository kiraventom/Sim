using Sim.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace Sim.Model;

internal class Raycaster
{
    public const double STEP = 0.00001;
    public static readonly Size DEFAULT_SIZE = new Size(STEP, STEP);

    private Map Map { get; }
    private HashSet<int> IgnoredIds { get; }

    private Point Current { get; set; }

    private RectI? _grid;
    private IEnumerable<Area> _areas;

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
            return RaycastResult.NO_HITS;

        var dir = ray.Normalize();
        double dist = 0;

        Current = start;

        while (dist <= rayLength)
        {
            Current += dir * STEP;
            dist += STEP;

            if (!Current.IsOnMap())
                break;

            if (RegisterHit(out var hit))
                return new RaycastResult(hit);
        }

        return RaycastResult.NO_HITS;
    }

    private bool RegisterHit(out RaycastHit hit)
    {
        var grid = Map.GetAreaGrid(new Rect(Current, DEFAULT_SIZE));
        if (grid != _grid)
        {
            _grid = grid;
            _areas = Map.GetAreasByGrid(grid);
        }

        foreach (var area in _areas)
        {
            foreach (var id in area.ObjectIds)
            {
                if (IgnoredIds.Contains(id))
                    continue;

                if (Map[id].Contains(Current))
                {
                    hit = new RaycastHit(id, Current);
                    return true;
                }
            }
        }

        hit = RaycastHit.INVALID;
        return false;
    }
}

