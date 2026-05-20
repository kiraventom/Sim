using Sim.Model;
using Sim.Model.Objects;

namespace Sim.Host;

internal class HumanFactory(Map map, MovableEvader movableEvader, RaycasterFactory raycasterFactory, PathBuilderFactory pathBuilderFactory, IdContainer idContainer)
{
    public Human Build()
    {
        var id = idContainer.NewId();
        var human = new Human(map, movableEvader, raycasterFactory, pathBuilderFactory, id);
        return human;
    }
}

