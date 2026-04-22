using Microsoft.Extensions.Logging;
using Sim.Geometry;
using System.Collections.Generic;

namespace Sim.Model;

internal class Map(ILogger<Map> logger)
{
    private readonly Dictionary<int, Point> _positions = [];

    public Point this[int id] => Positions.TryGetValue(id, out var point) ? point : Point.INVALID;

    public IReadOnlyDictionary<int, Point> Positions => _positions;

    public bool TryPlace(int id, Point pos)
    {
        if (_positions.TryGetValue(id, out var existingPos))
        {
            logger.LogDebug("Can't place {Id} to {Pos}, {Id} is already places at {ExistingPos}", id, pos, id, existingPos);
            return false;
        }

        _positions.Add(id, pos);
        return true;
    }

    public bool TryMove(int id, Point offset)
    {
        if (!_positions.TryGetValue(id, out var oldPos))
        {
            logger.LogDebug("Can't move {Id}, {Id} is not placed on map", id, id);
            return false;
        }

        var newPos = oldPos + offset;
        logger.LogTrace("Changed {Id} pos from {Old} to {New}, {Offset}", id, oldPos, newPos, offset);

        _positions[id] = newPos;
        return true;
    }
}
