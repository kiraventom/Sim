using Microsoft.Extensions.Logging;

namespace Sim.Model;

internal class RaycasterFactory(ILoggerFactory loggerFactory, World world, Map map)
{
    public Raycaster Build(int movableId)
    {
        return new Raycaster(loggerFactory.CreateLogger<Raycaster>(), world, map, movableId);
    }
}


