using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Sim.Geometry;
using Sim.Model.Objects;
using Sim.Model.Objects.Buildings;

namespace Sim.Model.Entities;

internal class EntityBuilder
{
    private readonly ILogger<EntityBuilder> logger;
    private readonly WorldSettings settings;
    private readonly World world;
    private readonly Map map;

    public EntityBuilder(ILogger<EntityBuilder> logger, WorldSettings settings, World world, Map map)
    {
        this.logger = logger;
        this.settings = settings;
        this.world = world;
        this.map = map;
    }

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
                case Human human when human.Path is Path path:
                    snapshot.Add(new HumanEntity(human.Id, absRect));
                    AddPath(snapshot, id, path);
                    break;

                case Human h:
                    snapshot.Add(new HumanEntity(h.Id, absRect));
                    break;

                case Obstacle o:
                    snapshot.Add(new ObstacleEntity(o.Id, absRect));
                    break;

                case House house:
                    snapshot.Add(new BuildingEntity(house.Id, absRect, BuildingType.House));
                    break;

                case Workplace workplace:
                    snapshot.Add(new BuildingEntity(workplace.Id, absRect, BuildingType.Workplace));
                    break;
            }

            AddAreas(snapshot, id, rect);
        }

        return snapshot;
    }

    private void AddPath(EntitySnapshot snapshot, int id, Path path)
    {
        var node = path.StartNode;

        while (true)
        {
            var nextNode = node.Next;
            if (nextNode is null)
                break;

            var pointA = node.Value.ToEntityPoint(settings);
            var pointB = nextNode.Value.ToEntityPoint(settings);
            snapshot.Add(new PathPartEntity(id, pointA, pointB, false, false));
            node = node.Next;
        }
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
