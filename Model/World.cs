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
            var rect = map.RandomFreeRect(obj.Size);
            if (rect.IsInvalid())
            {
                Logger.LogError("Failed to find free rect for size {Size}, skipping", obj.Size);
                continue;
            }

            if (map.TryPlace(obj.Id, rect))
            {
                _objects.Add(obj.Id, obj);
                Logger.LogTrace("Placed {Id} at {Rect}", obj.Id, rect);
            }
            else
            {
                Logger.LogError("Failed to place {Id} at {Rect}, skipping", obj.Id, rect);
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
