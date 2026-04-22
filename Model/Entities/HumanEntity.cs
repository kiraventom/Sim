using Sim.Geometry;
using Sim.Host;

namespace Sim.Model.Entities;

internal readonly struct HumanEntity(SizeI size, PointI pos) : IHumanEntity
{
    public int Priority => 10;
    public SizeI Size => size;
    public PointI Pos => pos;
}
