using System.Collections.Generic;
using Sim.Host;

namespace Sim.Model.Entities;

internal class EntityCache
{
    private readonly EntityBuilder _entBuilder;
    private IReadOnlyList<IEntity> _latest;

    public EntityCache(World world, EntityBuilder entBuilder)
    {
        world.AfterTick += UpdateCache;
        _entBuilder = entBuilder;
    }

    public int Size => _latest.Count;

    public IReadOnlyList<IEntity> GetEntities() => _latest;

    public IEntity GetEntity(int selectedEntityIndex) => _latest[selectedEntityIndex];

    private void UpdateCache()
    {
        _latest = _entBuilder.BuildEntities();
    }
}
