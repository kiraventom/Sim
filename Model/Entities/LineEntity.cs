using Sim.Geometry;
using Sim.Host;

namespace Sim.Model.Entities;

public readonly struct LineEntity(int id, PointI a, PointI b) : ILineEntity
{
    public int ObjectId => id;
    public PointI A => a;
    public PointI B => b;
}

