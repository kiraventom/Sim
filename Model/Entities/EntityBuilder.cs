using Microsoft.Extensions.Logging;
using Sim.Geometry;
using Sim.Model.Objects;

namespace Sim.Model.Entities;

internal class EntityBuilder(ILogger<EntityBuilder> logger, WorldSettings settings, World world, Map map)
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
                case Human h when h.CurrentPlan is Plan p:
                    snapshot.Add(new HumanEntity(h.Id, absRect));
                    snapshot.Add(new LineEntity(id, absRect.Pos, p.Target.ToAbsPoint(settings)));

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
        var grid = map.GetOverlappingGrid(rect);
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
