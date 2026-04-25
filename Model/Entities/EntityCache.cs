namespace Sim.Model.Entities;

internal class EntityCache
{
    private readonly EntityBuilder _entBuilder;
    private EntitySnapshot _frontBuffer = new();
    private EntitySnapshot _backBuffer = new();

    private readonly object _locker = new();

    public EntityCache(World world, EntityBuilder entBuilder)
    {
        world.AfterTick += UpdateCache;
        _entBuilder = entBuilder;
    }

    public void UpdateSnapshot(EntitySnapshot targetBuffer)
    {
        lock (_locker)
        {
            targetBuffer.CloneFrom(_frontBuffer);
        }
    }

    private void UpdateCache()
    {
        _entBuilder.UpdateSnapshot(_backBuffer);

        lock (_locker)
        {
            (_frontBuffer, _backBuffer) = (_backBuffer, _frontBuffer);
        }
    }
}
