using Sim.Geometry;

namespace Sim.Model.Objects.Buildings;

internal abstract class Building : SimObject
{
    protected Building(int id, Size size) : base(id, size)
    {
    }
}
