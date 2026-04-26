using Sim.Model;
using Sim.Model.Objects;

namespace Sim.Host;

internal class HumanFactory(Map map, PathBuilder pathfinder, IdContainer idContainer)
{
    public Human Build() => new Human(map, pathfinder, idContainer.NewId());
}

