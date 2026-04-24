using Sim.Geometry;
using Sim.Host;

namespace Sim.Model.Entities;

internal readonly struct HumanEntity(int id, RectI rect) : IHumanEntity
{
    public int Priority => 10;
    public int ObjectId => id;
    public RectI Rect => rect;
}
