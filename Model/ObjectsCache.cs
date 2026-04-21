using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Sim.Geometry;
using Sim.Host;

namespace Sim.Model;

internal class ObjectsCache(ILogger<ObjectsCache> logger, Map map)
{
    private IReadOnlyCollection<IObject> _latest;

    public IReadOnlyCollection<IObject> GetObjects() => _latest;

    public void UpdateCache()
    {
        // TODO #OBJECTS: Support other types
        // TODO #LARGEOBJECTS: Support size more than 1x1
        _latest = map.Positions
            .Select(x => new Object(size: new SizeI(1, 1), pos: x.Value, id: x.Key, type: ObjectType.Human))
            .Cast<IObject>()
            .ToList();
    }
}
