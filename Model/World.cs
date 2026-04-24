using Microsoft.Extensions.Logging;
using Sim.Geometry;
using Sim.Model.Objects;
using Sim.Utils;
using System;
using System.Collections.Generic;

namespace Sim.Model;

internal class World
{
    private ILogger<World> Logger { get; }
    private Map Map { get; }

    private readonly Dictionary<int, SimObject> _objects = [];

    internal IReadOnlyDictionary<int, SimObject> Objects => _objects;

    public event Action AfterTick;

    public World(ILogger<World> logger, Map map, WorldSettings settings)
    {
        Logger = logger;
        Map = map;

        var objs = GenerateObjects(settings);

        foreach (var obj in objs)
        {
            var pos = map.RandomFreePos();
            var rect = new Rect(pos, obj.Size);
            if (map.TryPlace(obj.Id, rect))
            {
                _objects.Add(obj.Id, obj);
                Logger.LogTrace("Placed {Id} of size {Size} at {Pos}", obj.Id, obj.Size, pos);
            }
            else
            {
                Logger.LogError("Failed to place {Id} of size {Size} at {Pos}, skipping", obj.Id, obj.Size, pos);
            }
        }
    }

    private IEnumerable<SimObject> GenerateObjects(WorldSettings settings)
    {
        for (int i = 0; i < settings.ObstaclesCount; ++i)
            yield return new Obstacle(Id.Generate(_objects));

        for (int i = 0; i < settings.HumansCount; ++i)
            yield return new Human(Id.Generate(_objects));
    }

    internal void Tick()
    {
        foreach (var obj in Objects.Values)
        {
            if (obj is Movable movable)
            {
                if (!movable.Move(Map))
                    Logger.LogError("Failed to move {Id} after {Att} attempts, skipping", obj.Id, Movable.ATTEMPTS_COUNT);
            }
        }

        AfterTick?.Invoke();
    }
}
