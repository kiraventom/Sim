using Microsoft.Extensions.Logging;
using Sim.Geometry;
using Sim.Model.Objects;
using System;
using System.Collections.Generic;

namespace Sim.Model;

internal class World(ILogger<World> logger, Map map)
{
    private readonly Dictionary<int, SimObject> _objects = [];

    internal IReadOnlyDictionary<int, SimObject> Objects => _objects;

    public event Action AfterTick;

    internal void Tick()
    {
        foreach (var obj in Objects.Values)
        {
            if (obj is Movable movable)
            {
                var pos = map[movable.Id].Pos;
                var moveOffset = movable.GetMoveOffset(pos);
                if (!map.TryMove(movable.Id, moveOffset))
                    logger.LogWarning("Failed to move {Id}, skipping", obj.Id);
            }
        }

        AfterTick?.Invoke();
    }

    internal void AddObject(SimObject obj) => _objects.Add(obj.Id, obj);
}
