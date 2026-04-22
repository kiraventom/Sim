using Microsoft.Extensions.Logging;
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

        for (int i = 0; i < settings.HumansCount; ++i)
        {
            var id = Id.Generate(_objects);
            var human = new Human(id);
            _objects.Add(id, human);
        }

        foreach (var id in _objects.Keys)
        {
            var pos = map.RandomFreePos();
            if (map.TryPlace(id, pos))
                Logger.LogTrace("Placed {Id} at {Pos}", id, pos);
            else
                Logger.LogError("Failed to place {Id} at {Pos}, skipping", id, pos);
        }
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
