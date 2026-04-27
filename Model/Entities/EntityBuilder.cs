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

            var absRect = rect.ToEntityRect(settings);

            switch (obj)
            {
                case Human h when h.Movement is Movement m:
                    snapshot.Add(new HumanEntity(h.Id, absRect));

                    var prevPoint = absRect.Center;
                    foreach (var point in m.Points)
                    {
                        var absPoint = point.ToEntityPoint(settings);
                        if (point == m.End)
                            snapshot.Add(new LineEntity(id, prevPoint, absPoint, isMainPath: true));
                        else
                            snapshot.Add(new LineEntity(id, prevPoint, absPoint, isAltPath: true));
                        prevPoint = absPoint;
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
                var areaSize = new Size(1.0 / Map.AREAS_COUNT, 1.0 / Map.AREAS_COUNT);
                var areaPos = new Point((double)c / Map.AREAS_COUNT + areaSize.Width / 2, (double)r / Map.AREAS_COUNT + areaSize.Height / 2);
                var areaRect = new Rect(areaPos, areaSize);
                var areaAbsRect = areaRect.ToEntityRect(settings);
                var area = new AreaEntity(id, areaAbsRect);
                snapshot.Add(area);
            }
        }
    }
}
