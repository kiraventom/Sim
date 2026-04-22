using Sim.Geometry;

namespace Sim.Host;

public interface IEntity
{
    int Priority { get; }
}

public interface IRectEntity : IEntity
{
    SizeI Size { get; }
    PointI Pos { get; }
}

public interface ILineEntity : IEntity
{
    PointI A { get; }
    PointI B { get; }
}

public interface IHumanEntity : IRectEntity
{
}
