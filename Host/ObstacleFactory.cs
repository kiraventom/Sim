using Sim.Model.Objects;

namespace Sim.Host;

internal class ObstacleFactory(IdContainer idContainer)
{
    public Obstacle Build() => new Obstacle(idContainer.NewId());
}

