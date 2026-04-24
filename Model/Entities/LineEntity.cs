using Sim.Geometry;
using Sim.Host;

namespace Sim.Model.Entities;

internal readonly struct LineEntity(int id, PointI a, PointI b) : ILineEntity
{
    public int Priority => 2;
    public int ObjectId => id;
    public PointI A => a;
    public PointI B => b;
}

