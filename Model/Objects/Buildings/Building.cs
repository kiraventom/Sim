using System.Collections.Generic;
using Sim.Geometry;

namespace Sim.Model.Objects.Buildings;

internal abstract class Building : SimObject
{
    private readonly List<Human> Occupants = [];

    protected Building(int id, Size size) : base(id, size)
    {
    }

    public bool AddOccupant(Human human)
    {
        if (Occupants.Contains(human))
            return false;

        Occupants.Add(human);
        return true;
    }

    internal bool RemoveOccupant(Human human)
    {
        return Occupants.Remove(human);
    }
}
