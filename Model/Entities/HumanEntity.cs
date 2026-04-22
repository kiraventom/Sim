using Sim.Geometry;
using Sim.Host;

namespace Sim.Model.Entities;

internal readonly struct HumanEntity(int id, SizeI size, PointI pos) : IHumanEntity
{
    public int Priority => 10;
    public int ObjectId => id;
    public SizeI Size => size;
    public PointI Pos => pos;
}
