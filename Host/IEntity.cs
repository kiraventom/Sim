using Sim.Geometry;

namespace Sim.Host;

public interface IEntity
{
    int Priority { get; }
    int ObjectId { get; }
}

public interface IRectEntity : IEntity
{
    RectI Rect { get; }
}

public interface ILineEntity : IEntity
{
    PointI A { get; }
    PointI B { get; }
}

public interface IHumanEntity : IRectEntity
{
}
