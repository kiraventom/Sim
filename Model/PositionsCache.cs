using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Extensions.Logging;
using Positions = System.Collections.Generic.IReadOnlyDictionary<int, Sim.Geometry.PointI>;

namespace Sim.Model;

internal class PositionsCache(ILogger<PositionsCache> logger, Map map)
{
    private const int MAX_CACHE_QUEUE_SIZE = 5;

    private ConcurrentQueue<Positions> Positions { get; } = [];

    public Positions GetPositions()
    {
        Positions.TryDequeue(out var result);
        return result;
    }

    public void UpdateCache()
    {
        if (Positions.Count > MAX_CACHE_QUEUE_SIZE)
            logger.LogWarning("Dropping old positions");

        while (Positions.Count > MAX_CACHE_QUEUE_SIZE)
            Positions.TryDequeue(out _);

        var positions = map.Positions.ToDictionary();

        Positions.Enqueue(positions);
    }
}

