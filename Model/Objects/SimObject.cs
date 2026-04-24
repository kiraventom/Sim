using Sim.Geometry;

namespace Sim.Model.Objects;

internal abstract class SimObject(int id)
{
    public int Id { get; } = id;
    public virtual Size Size { get; } = new Size(0.01, 0.01);
}

