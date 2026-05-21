using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Sim.Geometry;
using Sim.Model.Objects;

namespace Sim.Model.Entities;

internal class EntityBuilder
{
    private readonly ILogger<EntityBuilder> logger;
    private readonly WorldSettings settings;
    private readonly World world;
    private readonly Map map;
    private readonly MovableDetector detector;
    private readonly HashSet<int> _builtPaths = [];
    private readonly HashSet<int> _frozen = [];

    public EntityBuilder(ILogger<EntityBuilder> logger, WorldSettings settings, World world, Map map, MovableDetector detector, MovableEvader evader)
    {
        this.logger = logger;
        this.settings = settings;
        this.world = world;
        this.map = map;
        this.detector = detector;

        evader.PathBuilt += i => _builtPaths.Add(i);
        evader.Frozen += i => _frozen.Add(i);
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
                    snapshot.Add(new DetectionDistEntity(human.Id, detector.GetDetectionRect(human).ToEntityRect(settings)));
                    break;

                case Human h:
                    snapshot.Add(new HumanEntity(h.Id, absRect));
                    snapshot.Add(new DetectionDistEntity(h.Id, detector.GetDetectionRect(h).ToEntityRect(settings)));
                    break;

                case Obstacle o:
                    snapshot.Add(new ObstacleEntity(o.Id, absRect));
                    break;
            }

            AddAreas(snapshot, id, rect);
        }

        _builtPaths.Clear();
        _frozen.Clear();
        return snapshot;
    }

    private void AddPath(EntitySnapshot snapshot, int id, Path path)
    {
        var node = path.StartNode;
        var pathRebuilt = _builtPaths.Contains(id);
        var frozen = _frozen.Contains(id);

        while (true)
        {
            var nextNode = node.Next;
            if (nextNode is null)
                break;

            var pointA = node.Value.ToEntityPoint(settings);
            var pointB = nextNode.Value.ToEntityPoint(settings);
            snapshot.Add(new PathPartEntity(id, pointA, pointB, pathRebuilt, frozen));
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
