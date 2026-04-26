using System.Collections.Generic;
using Sim.Geometry;

namespace Sim.Model;

internal class RaycasterFactory(Map map)
{
    public Raycaster Build(IReadOnlyCollection<int> ignoredIds = null) => new Raycaster(map, ignoredIds);
}

