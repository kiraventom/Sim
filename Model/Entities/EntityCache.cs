using System.Collections.Generic;
using Sim.Host;

namespace Sim.Model.Entities;

internal class EntityCache
{
    private readonly EntityBuilder _entBuilder;
    private IReadOnlyCollection<IEntity> _latest;

    public EntityCache(World world, EntityBuilder entBuilder)
    {
        world.AfterTick += UpdateCache;
        _entBuilder = entBuilder;
    }

    public IReadOnlyCollection<IEntity> GetEntities() => _latest;

    private void UpdateCache()
    {
        _latest = _entBuilder.BuildEntities();
    }
}
