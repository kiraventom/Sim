using Microsoft.Extensions.Logging;
using Sim.Geometry;
using System.Collections.Generic;

namespace Sim.Model;

internal class Map(ILogger<Map> logger, WorldSettings settings)
{
    private readonly Dictionary<int, PointI> _positions = [];

    public SizeI Size { get; } = new SizeI(settings.MapWidth, settings.MapHeight);
    private readonly int[,] _map = new int[settings.MapWidth, settings.MapHeight];

    public PointI this[int id] => Positions.TryGetValue(id, out var point) ? point : PointI.INVALID;
    public int this[int x, int y] => GetPoint(x, y);
    public int this[PointI p] => GetPoint(p.X, p.Y);

    public IReadOnlyDictionary<int, PointI> Positions => _positions;

    public bool TryPlace(int id, PointI pos)
    {
        if (this[pos] == SpecialCell.INVALID)
        {
            logger.LogTrace("Can't place {Id} to {Pos}, {Pos} is invalid", id, pos, pos);
            return false;
        }

        if (this[pos] != SpecialCell.EMPTY)
        {
            logger.LogTrace("Can't place {Id} to {Pos}, {Pos} is not empty", id, pos, pos);
            return false;
        }

        if (_positions.TryGetValue(id, out var existingPos))
        {
            logger.LogTrace("Can't place {Id} to {Pos}, {Id} is already places at {ExistingPos}", id, pos, id, existingPos);
            return false;
        }

        _map[pos.X, pos.Y] = id;
        _positions.Add(id, pos);
        return true;
    }

    public bool TryMove(int id, PointI offset)
    {
        if (!_positions.TryGetValue(id, out var oldPos))
        {
            logger.LogTrace("Can't move {Id}, {Id} is not placed on map", id, id);
            return false;
        }

        var newPos = oldPos + offset;

        if (this[newPos] == SpecialCell.INVALID)
        {
            logger.LogTrace("Can't move {Id} to {Pos}, {Pos} is invalid", id, newPos, newPos);
            return false;
        }

        if (this[newPos] != SpecialCell.EMPTY)
        {
            logger.LogTrace("Can't move {Id} from {OldPos} to {NewPos}, {NewPos} is occupied by {OccId}", id, oldPos, newPos, newPos, this[newPos]);
            return false;
        }

        _map[oldPos.X, oldPos.Y] = SpecialCell.EMPTY;
        _map[newPos.X, newPos.Y] = id;
        _positions[id] = newPos;

        return true;
    }

    private int GetPoint(int x, int y)
    {
        if (x < 0 || y < 0)
            return SpecialCell.INVALID;

        if (x >= Size.Width || y >= Size.Height)
            return SpecialCell.INVALID;

        return _map[x, y];
    }
}

