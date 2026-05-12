using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Sim.Geometry;
using Sim.Model.Objects;

namespace Sim.Model;

internal class MovableDetector(ILogger<MovableDetector> logger, Map map, World world)
{
    private const double DETECTION_DISTANCE_MODIFIER = 5.0;

    public IEnumerable<int> Detect(int id, Point offset)
    {
        var movableRect = map[id];
        var detectionDist = movableRect.Size * DETECTION_DISTANCE_MODIFIER;
        var areaSize = new Size(1.0 / Map.AREAS_COUNT, 1.0 / Map.AREAS_COUNT);

        var grid = map.GetAreaGrid(movableRect);
        
        int minCol = grid.Left;
        int maxCol = grid.Right;
        int minRow = grid.Top;
        int maxRow = grid.Bottom;

        if (offset.X > 0) 
            maxCol = Math.Min(Map.AREAS_COUNT - 1, maxCol + 1);
        else if (offset.X < 0) 
            minCol = Math.Max(0, minCol - 1);

        if (offset.Y > 0) 
            maxRow = Math.Min(Map.AREAS_COUNT - 1, maxRow + 1);
        else if (offset.Y < 0) 
            minRow = Math.Max(0, minRow - 1);

        var searchGrid = new RectI(new PointI(minCol, minRow), new SizeI(maxCol - minCol, maxRow - minRow));
        
        foreach (var area in map.GetAreasByGrid(searchGrid))
        {
            foreach (var otherId in area.ObjectIds)
            {
                if (otherId == id) 
                    continue;

                if (world.Objects.TryGetValue(otherId, out var obj) && obj is Movable)
                {
                    var otherRect = map[otherId];
                    var distX = Math.Abs(movableRect.Pos.X - otherRect.Pos.X);
                    var distY = Math.Abs(movableRect.Pos.Y - otherRect.Pos.Y);

                    if (distX <= detectionDist.Width && distY <= detectionDist.Height)
                        yield return otherId;
                }
            }
        }
    }
}
