using Sim.Geometry;
using Sim.Model.Objects.Buildings;
using Sim.Utils;

namespace Sim.Model.Objects;

internal class Human : Movable
{
    private Map Map { get; } // TEMP

    public House House { get; private set; }
    public bool Homeless => House is null;

    public Workplace Job { get; private set; }
    public bool Jobless => Job is null;

    public Human(Map map, RaycasterFactory raycasterFactory, PathBuilderFactory pathBuilderFactory, int id) : base(raycasterFactory, pathBuilderFactory, id, new Size(0.005, 0.005))
    {
        Map = map;

        const double SpeedModMin = 0.0015;
        const double SpeedModMax = 0.003;
        Speed = RND.Double(SpeedModMin, SpeedModMax);
    }

    public bool AssignHouse(House house, bool force = false)
    {
        if (IsSemaphoreSet())
            return true;

        using (_ = Semaphore())
        {
            if (House is not null)
            {
                if (!force)
                    return false;

                House.RemoveOwner();
            }

            House = house;
            return House.AssignOwner(this, force);
        }
    }

    public bool RemoveHouse()
    {
        if (IsSemaphoreSet())
            return true;

        using (_ = Semaphore())
        {
            if (House is null)
                return false;

            House.RemoveOwner();
            House = null;
            return true;
        }
    }

    protected override Point GetNewTarget(Point pos)
    {
        return Map.RandomFreeRect(Size).Pos;
    }
}
