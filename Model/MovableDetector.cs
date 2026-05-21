using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Sim.Geometry;
using Sim.Model.Objects;

namespace Sim.Model;

internal class MovableDetector(ILogger<MovableDetector> logger, Map map, World world)
{
    private const double DETECTION_DISTANCE_MODIFIER = 5.0;

    public bool PathsIntersect(int id1, int id2)
    {
        if (world.Objects[id1] is not Movable m1 || m1.Path == null)
            return true;

        if (world.Objects[id2] is not Movable m2 || m2.Path == null)
            return true;

        var p1 = m1.Path.TargetNode.Previous.Value;
        var p2 = m1.Path.TargetPoint;
        var p3 = m2.Path.TargetNode.Previous.Value;
        var p4 = m2.Path.TargetPoint;

        double d = (p2.X - p1.X) * (p4.Y - p3.Y) - (p2.Y - p1.Y) * (p4.X - p3.X);
        if (Math.Abs(d) < 1e-9) 
            return false;

        double u = ((p3.X - p1.X) * (p4.Y - p3.Y) - (p3.Y - p1.Y) * (p4.X - p3.X)) / d;
        double v = ((p3.X - p1.X) * (p2.Y - p1.Y) - (p3.Y - p1.Y) * (p2.X - p1.X)) / d;

        return u >= 0.0 && u <= 1.0 && v >= 0.0 && v <= 1.0;
    }

    public Rect GetDetectionRect(Movable movable)
    {
        var movableRect = map[movable.Id];
        var detectionDist = movableRect.Size * DETECTION_DISTANCE_MODIFIER;
        return new Rect(movableRect.Pos, detectionDist);
    }

    public IEnumerable<int> Detect(int id)
    {
        if (world.Objects[id] is not Movable movable)
            yield break;

        var detectionRect = GetDetectionRect(movable);
        var movableRect = map[id];

        var grid = map.GetAreaGrid(detectionRect);
        var areaSize = new Size(1.0 / Map.AREAS_COUNT, 1.0 / Map.AREAS_COUNT);

        foreach (var area in map.GetAreasByGrid(grid))
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

                    if (distX > detectionRect.Width || distY > detectionRect.Height)
                        continue;

                    yield return otherId;
                }
            }
        }
    }
}
