using Sim.Model;
using Sim.Model.Objects;

namespace Sim.Host;

internal class HumanFactory(Map map, PathBuilderFactory pathBuilderFactory, IdContainer idContainer)
{
    public Human Build() => new Human(map, pathBuilderFactory, idContainer.NewId());
}

