using Sim.Geometry;
using Sim.Host;

namespace Sim.Model.Entities;

internal readonly struct LineEntity(PointI a, PointI b) : ILineEntity
{
    public int Priority => 1;
    public PointI A => a;
    public PointI B => b;
}

