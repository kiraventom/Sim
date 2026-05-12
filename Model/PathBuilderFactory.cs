using Microsoft.Extensions.Logging;
using Sim.Geometry;

namespace Sim.Model;

internal class PathBuilderFactory(ILoggerFactory loggerFactory, Map map)
{
    internal PathBuilder Build(int id, Size size, RaycasterFactory raycasterFactory)
    {
        return new PathBuilder(loggerFactory.CreateLogger<PathBuilder>(), map, raycasterFactory, id, size);
    }
}

