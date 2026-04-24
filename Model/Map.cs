using Microsoft.Extensions.Logging;
using Sim.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sim.Model;

internal class Map
{
    private const int AREAS_COUNT = 5;

    private readonly Dictionary<int, Rect> _rects = [];

    private readonly HashSet<int>[,] _areas = new HashSet<int>[AREAS_COUNT, AREAS_COUNT];

    private ILogger<Map> Logger { get; }

    public Rect this[int id] => Rects.TryGetValue(id, out var rect) ? rect : Rect.INVALID;

    public Map(ILogger<Map> logger)
    {
        Logger = logger;

        for (int i = 0; i < AREAS_COUNT; ++i)
        {
            for (int j = 0; j < AREAS_COUNT; ++j)
            {
                _areas[i, j] = new HashSet<int>();
            }
        }
    }

    public IReadOnlyDictionary<int, Rect> Rects => _rects;

    public bool TryPlace(int id, Rect rect)
    {
        if (_rects.TryGetValue(id, out var existingRect))
        {
            Logger.LogDebug("Can't place {Id} to {Pos}, {Id} is already places at {ExistingPos}", id, rect.Pos, id, existingRect.Pos);
            return false;
        }

        var area = GetArea(rect.Pos);

        if (!CanPlace(area, rect))
        {
            Logger.LogDebug("Can't place {Id} to {Pos}, place is taken", id, rect.Pos);
            return false;
        }

        _rects.Add(id, rect);
        area.Add(id);
        return true;
    }

    public bool TryMove(int id, Point offset)
    {
        if (!_rects.TryGetValue(id, out var oldRect))
        {
            Logger.LogDebug("Can't move {Id}, {Id} is not placed on map", id, id);
            return false;
        }

        var newRect = oldRect.Offset(offset);

        var oldArea = GetArea(oldRect.Pos);
        var area = GetArea(newRect.Pos);

        if (!CanPlace(area, newRect, id))
        {
            Logger.LogDebug("Can't move {Id} to {Pos}, place is taken", id, newRect.Pos);
            return false;
        }

        Logger.LogTrace("Changed {Id} pos from {Old} to {New}, {Offset}", id, oldRect.Pos, newRect.Pos, offset);

        if (oldArea != area)
            oldArea.Remove(id);

        _rects[id] = newRect;
        area.Add(id);
        return true;
    }

    private HashSet<int> GetArea(Point pos)
    {
        var row = (int)Math.Floor(pos.X * AREAS_COUNT);
        var column = (int)Math.Floor(pos.Y * AREAS_COUNT);

        if (row < 0)
        {
            Logger.LogError("Math.Floor({X} * {AreaCount}) == {row}, which is < 0", pos.X, AREAS_COUNT, row);
            return null;
        }

        if (column < 0)
        {
            Logger.LogError("Math.Floor({Y} * {AreaCount}) == {column}, which is < 0", pos.Y, AREAS_COUNT, column);
            return null;
        }

        if (row >= AREAS_COUNT)
        {
            Logger.LogError("Math.Floor({X} * {AreaCount}) == {row}, which is >= {AreaCount}", pos.X, AREAS_COUNT, row, AREAS_COUNT);
            return null;
        }

        if (column >= AREAS_COUNT)
        {
            Logger.LogError("Math.Floor({Y} * {AreaCount}) == {column}, which is >= {AreaCount}", pos.Y, AREAS_COUNT, column, AREAS_COUNT);
            return null;
        }

        return _areas[row, column];
    }

    private bool CanPlace(HashSet<int> area, Rect rect, int? id = null)
    {
        foreach (var i in area)
        {
            if (id.HasValue && id.Value == i)
                continue;

            if (_rects[i].Intersects(rect))
                return false;
        }
    
        return true;
    }
}
