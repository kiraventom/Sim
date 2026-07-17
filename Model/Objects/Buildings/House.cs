using Sim.Geometry;

namespace Sim.Model.Objects.Buildings;

internal class House : Building
{
    public Human Owner { get; private set; }

    public House(int id) : base(id, new Size(0.05, 0.05))
    {
    }

    public bool AssignOwner(Human human, bool force = false)
    {
        if (IsSemaphoreSet())
            return true;

        using (_ = Semaphore())
        {
            if (Owner is not null)
            {
                if (!force)
                    return false;

                Owner.RemoveHouse();
            }

            Owner = human;
            return Owner.AssignHouse(this, force);
        }
    }

    public bool RemoveOwner()
    {
        if (IsSemaphoreSet())
            return true;

        using (_ = Semaphore())
        {
            if (Owner is null)
                return false;

            Owner.RemoveHouse();
            Owner = null;
            return true;
        }
    }
}

