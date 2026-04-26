using Microsoft.Extensions.Logging;
using Sim.Geometry;
using System;
using System.Collections.Generic;

namespace Sim.Model;

internal class Map
{
    public const int AREAS_COUNT = 15;

    private readonly Dictionary<int, Rect> _rects = [];

    private readonly Area[,] _areas = new Area[AREAS_COUNT, AREAS_COUNT];

    private ILogger<Map> Logger { get; }

    public Rect this[int id] => Rects.TryGetValue(id, out var rect) ? rect : Rect.INVALID;

    public Map(ILogger<Map> logger)
    {
        Logger = logger;

        for (int i = 0; i < AREAS_COUNT; ++i)
            for (int j = 0; j < AREAS_COUNT; ++j)
                _areas[i, j] = new Area();
    }

    public IReadOnlyDictionary<int, Rect> Rects => _rects;

    public bool TryPlace(int id, Rect rect)
    {
        if (_rects.TryGetValue(id, out var existingRect))
        {
            Logger.LogDebug("Can't place {Id} to {Pos}, {Id} is already places at {ExistingPos}", id, rect.Center, id, existingRect.Center);
            return false;
        }

        if (!Rect.EnsureValid(ref rect))
            Logger.LogInformation("{Rect} was adjusted to be valid", rect);

        var grid = GetAreaGrid(rect);

        if (!CanPlace(grid, rect))
        {
            Logger.LogDebug("Can't place {Id} to {Pos}, place is taken", id, rect.Center);
            return false;
        }

        _rects.Add(id, rect);

        for (int r = grid.Top; r <= grid.Bottom; ++r)
            for (int c = grid.Left; c <= grid.Right; ++c)
                _areas[r, c].Add(id);

        return true;
    }

    public bool TryMove(int id, Point offset)
    {
        if (offset.IsZero())
            return true;
        
        if (!_rects.TryGetValue(id, out var oldRect))
        {
            Logger.LogDebug("Can't move {Id}, {Id} is not placed on map", id, id);
            return false;
        }

        var newRect = oldRect.Offset(offset);

        if (!Rect.EnsureValid(ref newRect))
            Logger.LogInformation("{NewRect} was adjusted to be valid", newRect);

        var oldGrid = GetAreaGrid(oldRect);
        var newGrid = GetAreaGrid(newRect);

        if (!CanPlace(newGrid, newRect, id))
        {
            Logger.LogDebug("Can't move {Id} to {Pos}, place is taken", id, newRect.Center);
            return false;
        }

        Logger.LogTrace("Changed {Id} pos from {Old} to {New}, {Offset}", id, oldRect.Center, newRect.Center, offset);

        // TODO: move those to RectI
        for (int r = oldGrid.Top; r <= oldGrid.Bottom; ++r)
        {
            for (int c = oldGrid.Left; c <= oldGrid.Right; ++c)
            {
                bool inNew = r >= newGrid.Top && r <= newGrid.Bottom && c >= newGrid.Left && c <= newGrid.Right;
                if (!inNew)
                    _areas[r, c].Remove(id);
            }
        }

        _rects[id] = newRect;

        // TODO: move those to RectI
        for (int r = newGrid.Top; r <= newGrid.Bottom; ++r)
        {
            for (int c = newGrid.Left; c <= newGrid.Right; ++c)
            {
                bool inOld = r >= oldGrid.Top && r <= oldGrid.Bottom && c >= oldGrid.Left && c <= oldGrid.Right;
                if (!inOld)
                    _areas[r, c].Add(id);
            }
        }

        return true;
    }

    internal RectI GetAreaGrid(Rect rect)
    {
        int startRow = Math.Clamp((int)Math.Floor(rect.Top * AREAS_COUNT), 0, AREAS_COUNT - 1);
        int endRow = Math.Clamp((int)Math.Floor(rect.Bottom * AREAS_COUNT), 0, AREAS_COUNT - 1);
        int startCol = Math.Clamp((int)Math.Floor(rect.Left * AREAS_COUNT), 0, AREAS_COUNT - 1);
        int endCol = Math.Clamp((int)Math.Floor(rect.Right * AREAS_COUNT), 0, AREAS_COUNT - 1);

        return new RectI(new PointI(startCol, startRow), new SizeI(endCol - startCol, endRow - startRow));
    }

    internal IEnumerable<Area> GetAreasByGrid(RectI grid)
    {
        for (int r = grid.Top; r <= grid.Bottom; ++r)
        {
            for (int c = grid.Left; c <= grid.Right; ++c)
            {
                yield return _areas[r, c];
            }
        }
    }

    internal bool CanPlace(RectI grid, Rect rect, int? id = null)
    {
        foreach (var area in GetAreasByGrid(grid))
        {
            foreach (var existingId in area.ObjectIds)
            {
                if (id.HasValue && id.Value == existingId)
                    continue;

                if (_rects[existingId].Intersects(rect))
                    return false;
            }
        }

        return true;
    }
}
