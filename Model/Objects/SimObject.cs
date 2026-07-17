using Sim.Geometry;

namespace Sim.Model.Objects;

internal abstract class SimObject(int id, Size size)
{
    private readonly Semaphore _semaphore = new();

    public int Id { get; } = id;
    public Size Size { get; } = size;
    public virtual bool HasCollision => false; // TODO Remove collision altogether, it's useless

    public virtual Rect RandStartRect(Map map) => map.RandomFreeRect(Size);

    protected bool IsSemaphoreSet() => _semaphore.IsSet(this);
    protected SemaphoreObject Semaphore() => _semaphore.SetOnce(this);
}

