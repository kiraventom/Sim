using Microsoft.Extensions.Logging;
using Sim.Geometry;
using Sim.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Positions = System.Collections.Generic.IReadOnlyDictionary<int, Sim.Geometry.PointI>;

namespace Sim.Model;

public class World
{
    private readonly IReadOnlyDictionary<int, Human> _humans;
    private ILogger<World> Logger { get; }
    private Map Map { get; }
    private PositionsCache Cache { get; }

    public Size Size => Map.Size;

    public World(ILogger<World> logger, Map map, PositionsCache cache, WorldSettings settings)
    {
        Logger = logger;
        Map = map;
        Cache = cache;

        var humans = new Dictionary<int, Human>();
        for (int i = 0; i < settings.HumansCount; ++i)
        {
            var id = Id.Generate(humans);
            var human = new Human(id);
            humans.Add(id, human);
        }

        _humans = humans;

        foreach (var humanId in humans.Keys)
        {
            var pos = map.RandomFreePos();
            if (map.TryPlace(humanId, pos))
                Logger.LogDebug("Placed {Id} at {Pos}", humanId, pos);
            else
                Logger.LogError("Failed to place {Id} at {Pos}, skipping", humanId, pos);
        }
    }

    public Positions GetPositions() => Cache.GetPositions();

    internal void Tick()
    {
        foreach (var human in _humans.Values)
        {
            for (int attempt = 0; ; ++attempt)
            {
                var offset = human.GetMoveOffset(Map, forceNew: attempt != 0);
                if (offset.IsZero())
                    break;

                if (Map.TryMove(human.Id, offset))
                    break;

                if (attempt == 10)
                {
                    Logger.LogError("Failed to move {Id}, skipping", human.Id);
                    break;
                }
            }
        }

        Cache.UpdateCache();
    }
}
