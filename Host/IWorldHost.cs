using System.Collections.Generic;
using Sim.Geometry;

namespace Sim.Host;

public interface IWorldHost
{
    SizeI WorldSize { get; }
    void TogglePause();
    IReadOnlyCollection<IObject> GetObjects();
}
