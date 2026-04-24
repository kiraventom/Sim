using Sim.Geometry;
using Sim.Host;

namespace Sim.Model.Entities;

internal readonly struct ObstacleEntity(int id, RectI rect) : IObstacleEntity
{
    public int Priority => 1;
    public int ObjectId => id;
    public RectI Rect => rect;
}

