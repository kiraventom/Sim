using Positions = System.Collections.Generic.IReadOnlyDictionary<int, Sim.Geometry.PointI>;
using Sim.Geometry;

namespace Sim.Host;

public interface IWorldHost
{
    SizeI WorldSize { get; }
    void TogglePause();
    Positions GetPositions();
}


