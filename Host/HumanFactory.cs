using Sim.Model;
using Sim.Model.Objects;

namespace Sim.Host;

internal class HumanFactory(Map map, RaycasterFactory raycasterFactory, PathBuilderFactory pathBuilderFactory, IdContainer idContainer)
{
    public Human Build()
    {
        var id = idContainer.NewId();
        var human = new Human(map, raycasterFactory, pathBuilderFactory, id);
        return human;
    }
}
