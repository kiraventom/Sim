using Sim.Geometry;
using Sim.Host;

namespace Sim.Model;

internal readonly struct Object(SizeI size, PointI pos, int id, ObjectType type) : IObject
{
    public SizeI Size => size;
    public PointI Pos => pos;
    public int Id => id;
    public ObjectType Type => type;
}

