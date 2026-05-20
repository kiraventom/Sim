using Sim.Geometry;
using Sim.Host;

namespace Sim.Model.Entities;

public readonly struct DetectionDistEntity(int id, RectI rect) : IRectEntity
{
    public RectI Rect => rect;
    public int ObjectId => id;
}

