using Microsoft.Extensions.Logging;
using Sim.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sim.Model;

internal class Map
{
    private const int AREAS_COUNT = 3;

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

        if (!Rect.EnsureValid(ref rect))
            Logger.LogWarning("{Rect} was adjusted to be valid", rect);

        var areaIndexes = GetAreaIndexes(rect.Pos);
        var area = GetArea(areaIndexes);

        if (!CanPlace(areaIndexes, rect))
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

        if (!Rect.EnsureValid(ref newRect))
            Logger.LogWarning("{NewRect} was adjusted to be valid", newRect);

        var oldArea = GetArea(oldRect.Pos);

        var areaIndexes = GetAreaIndexes(newRect.Pos);
        var area = GetArea(areaIndexes);

        if (!CanPlace(areaIndexes, newRect, id))
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

    private HashSet<int> GetArea((int row, int column) x) => GetArea(x.row, x.column);
    private HashSet<int> GetArea(int row, int column) => _areas[row, column];
    private HashSet<int> GetArea(Point pos) => GetArea(GetAreaIndexes(pos));

    private (int row, int column) GetAreaIndexes(Point pos)
    {
        var row = (int)Math.Floor(pos.X * AREAS_COUNT);
        var column = (int)Math.Floor(pos.Y * AREAS_COUNT);

        return (row, column);
    }

    private IEnumerable<HashSet<int>> GetAdjacentAreas(int row, int column)
    {
        for (int r = row - 1; r < AREAS_COUNT && r <= row + 1; ++r)
        {
            if (r < 0)
                continue;

            for (int c = column - 1; c < AREAS_COUNT && c <= column + 1; ++c)
            {
                if (c < 0)
                    continue;

                yield return GetArea(r, c);
            }
        }
    }

    private bool CanPlace((int row, int column) x, Rect rect, int? id = null) => CanPlace(x.row, x.column, rect, id);

    private bool CanPlace(int row, int column, Rect rect, int? id = null)
    {
        var areas = GetAdjacentAreas(row, column);
        foreach (var i in areas.SelectMany(i => i))
        {
            if (id.HasValue && id.Value == i)
                continue;

            if (_rects[i].Intersects(rect))
                return false;
        }
    
        return true;
    }
}
