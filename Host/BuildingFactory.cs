using Sim.Model.Objects;
using Sim.Model.Objects.Buildings;

namespace Sim.Host;

internal class BuildingFactory(IdContainer idContainer)
{
    public House BuildHouse() => new House(idContainer.NewId());
}


