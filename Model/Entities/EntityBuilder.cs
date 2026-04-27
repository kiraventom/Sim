using System;
using Microsoft.Extensions.Logging;
using Sim.Geometry;
using Sim.Model.Objects;

namespace Sim.Model.Entities;

internal class EntityBuilder(ILogger<EntityBuilder> logger, WorldSettings settings, World world, Map map, Pathfinder DBG_Pathfinder)
{
    public EntitySnapshot UpdateSnapshot(EntitySnapshot snapshot)
    {
        snapshot.Clear();

        foreach (var (id, rect) in map.Rects)
        {
            if (!world.Objects.TryGetValue(id, out var obj))
            {
                logger.LogError("Object {Id} is present on map, but not found in world", id);
                continue;
            }

            var absRect = rect.ToAbsRect(settings);

            switch (obj)
            {
                case Human h when h.Movement is Movement m:
                    snapshot.Add(new HumanEntity(h.Id, absRect));
                    snapshot.Add(new LineEntity(id, absRect.Pos, m.GetTarget().ToAbsPoint(settings), isMainPath: true));

                    // DBG
                    {
                        var adjustedTarget = DBG_Pathfinder.GetAdjustedTarget(h, m.GetTarget());
                        if (adjustedTarget != m.GetTarget())
                        {
                            snapshot.Add(new LineEntity(id, absRect.Pos, adjustedTarget.ToAbsPoint(settings), isAltPath: true));
                        }

                        var maxDist = Pathfinder.GetMaxPushDistance(h.Size);
                        var grid = DBG_Pathfinder.GetLookAroundGrid(rect);
                        var areas = map.GetAreasByGrid(grid);
                        foreach (var area in areas)
                        {
                            var ids = area.ObjectIds;
                            foreach (var objId in ids)
                            {
                                if (objId == id)
                                    continue;

                                var objRect = map[objId];
                                var (a, b) = Rect.GetDirectVector(rect, objRect);
                                var distVec = (a - b);
                                if (distVec.Length > maxDist)
                                    continue;

                                snapshot.Add(new LineEntity(id, a.ToAbsPoint(settings), b.ToAbsPoint(settings)));
                            }
                        }
                    }
                    
                    break;

                case Human h:
                    snapshot.Add(new HumanEntity(h.Id, absRect));
                    break;

                case Obstacle o:
                    snapshot.Add(new ObstacleEntity(o.Id, absRect));
                    break;
            }

            AddAreas(snapshot, id, rect);
        }

        return snapshot;
    }

    private void AddAreas(EntitySnapshot snapshot, int id, Rect rect)
    {
        var grid = map.GetAreaGrid(rect);
        for (int r = grid.Top; r <= grid.Bottom; ++r)
        {
            for (int c = grid.Left; c <= grid.Right; ++c)
            {
                var areaPos = new Point((double)c / Map.AREAS_COUNT, (double)r / Map.AREAS_COUNT);
                var areaSize = new Size(1.0 / Map.AREAS_COUNT, 1.0 / Map.AREAS_COUNT);
                var areaRect = new Rect(areaPos, areaSize);
                var areaAbsRect = areaRect.ToAbsRect(settings);
                var area = new AreaEntity(id, areaAbsRect);
                snapshot.Add(area);
            }
        }
    }
}
