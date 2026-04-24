namespace Sim.Model.Entities;

internal class EntityCache
{
    private readonly EntityBuilder _entBuilder;
    private EntitySnapshot _latest = new();

    public EntityCache(World world, EntityBuilder entBuilder)
    {
        world.AfterTick += UpdateCache;
        _entBuilder = entBuilder;
    }

    public EntitySnapshot GetSnapshot() => _latest;

    private void UpdateCache()
    {
        _latest = _entBuilder.BuildSnapshot();
    }
}
