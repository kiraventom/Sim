using Microsoft.Extensions.Logging;
using Sim.Model.Objects;
using System;
using System.Collections.Generic;

namespace Sim.Model;

internal class World
{
    private readonly Dictionary<int, SimObject> _objects = [];

    public World(ILogger<World> logger, Map map)
    {
        Logger = logger;
        Map = map;
        Map.HasCollision = id => _objects.TryGetValue(id, out var obj) && obj.HasCollision;
    }

    public ILogger<World> Logger { get; }
    public Map Map { get; }

    internal IReadOnlyDictionary<int, SimObject> Objects => _objects;

    public event Action AfterTick;

    internal void Tick()
    {
        foreach (var obj in Objects.Values)
        {
            if (obj is Movable movable)
            {
                var pos = Map[movable.Id].Pos;
                var moveOffset = movable.GetMoveOffset(pos);
                if (!Map.TryMove(movable.Id, moveOffset))
                    Logger.LogWarning("Failed to move {Id}, skipping", obj.Id);
            }
        }

        AfterTick?.Invoke();
    }

    internal void AddObject(SimObject obj) => _objects.Add(obj.Id, obj);
}
