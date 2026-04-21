using Sim.Geometry;

namespace Sim.Host;

public interface IObject
{
    SizeI Size { get; }
    PointI Pos { get; }
    int Id { get; }
    ObjectType Type { get; }
}


