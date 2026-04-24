using Sim.Geometry;
using Sim.Host;

namespace Sim.Model.Entities;

public readonly struct AreaEntity(int id, RectI rect) : IRectEntity
{
    public int ObjectId => id;
    public RectI Rect => rect;
}

