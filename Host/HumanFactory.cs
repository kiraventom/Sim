using Sim.Model;
using Sim.Model.Objects;

namespace Sim.Host;

internal class HumanFactory(PlanFactory planFactory, RaycasterFactory raycasterFactory, PathBuilderFactory pathBuilderFactory, IdContainer idContainer)
{
    public Human Build()
    {
        var id = idContainer.NewId();
        var plan = planFactory.Build();
        var human = new Human(plan, raycasterFactory, pathBuilderFactory, id);
        return human;
    }
}
