using Sim.Geometry;
using Sim.Host;

namespace Sim.Model.Entities;

public readonly struct PathPartEntity(int id, PointI a, PointI b, bool isRebuilt = false, bool isFrozen = false) : ILineEntity
{
    public int ObjectId => id;
    public PointI A => a;
    public PointI B => b;
    public bool IsRebuilt => isRebuilt;
    public bool IsFrozen => isFrozen;
}

