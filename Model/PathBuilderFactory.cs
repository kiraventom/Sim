using Microsoft.Extensions.Logging;
using Sim.Geometry;

namespace Sim.Model;

internal class PathBuilderFactory(ILoggerFactory loggerFactory, Map map)
{
    internal PathBuilder Build(int id, Size size)
    {
        var raycaster = new Raycaster(map, [id]);
        return new PathBuilder(loggerFactory.CreateLogger<PathBuilder>(), map, raycaster, id, size);
    }
}

