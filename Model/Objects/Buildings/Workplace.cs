using Sim.Geometry;

namespace Sim.Model.Objects.Buildings;

internal abstract class Workplace : Building
{
    protected Workplace(int id, Size size) : base(id, size)
    {
    }
}

