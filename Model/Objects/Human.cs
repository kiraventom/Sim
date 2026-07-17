using System;
using System.Collections.Generic;
using Sim.Geometry;
using Sim.Model.Objects.Buildings;
using Sim.Utils;

namespace Sim.Model.Objects;

internal abstract class HumanTask(World world, Map map)
{
    protected World World { get; } = world;
    protected Map Map { get; } = map;

    public abstract Point GetTargetPoint(Human human);
}

internal class WanderTask(World world, Map map) : HumanTask(world, map)
{
    private Point? _point;

    public override Point GetTargetPoint(Human human)
    {
        if (!human.IsOutside)
            human.Leave();

        return _point ??= Map.RandomFreeRect(human.Size).Pos;
    }
}

internal class MoveTask(World world, Map map, Point point) : HumanTask(world, map)
{
    public override Point GetTargetPoint(Human human)
    {
        if (!human.IsOutside)
            human.Leave();

        return point;
    }
}

internal class EnterHouseTask(World world, Map map) : HumanTask(world, map)
{
    public override Point GetTargetPoint(Human human)
    {
        var houseRect = Map[human.House.Id];
        var humanRect = Map[human.Id];
        var housePos = houseRect.Pos;
        var humanPos = humanRect.Pos;
        var dir = humanPos - housePos;
        var dist = dir.Length;

        var humanRadius = Math.Sqrt(Math.Pow(humanRect.Width / 2, 2) + Math.Pow(humanRect.Height / 2, 2));
        var houseRadius = Math.Sqrt(Math.Pow(houseRect.Width / 2, 2) + Math.Pow(houseRect.Height / 2, 2));
        var minDist = (humanRadius + houseRadius) * 1.1; 

        if (dist <= minDist)
        {
            human.Enter(human.House);
            return humanPos;
        }

        return housePos;
    }
}

internal class PlanFactory(Map map, World world)
{
    public Plan Build() => new Plan(world, map);
}

internal class Plan
{
    private readonly Queue<HumanTask> _tasks = [];

    public World World { get; }
    public Map Map { get; }

    public Plan(World world, Map map)
    {
        World = world;
        Map = map;

        RefillTasks();
    }

    public HumanTask GetTask(Human human)
    {
        var task = _tasks.Peek();
        var humanPos = Map[human.Id].Pos;
        var targetPos = task.GetTargetPoint(human);
        if (CMP.Equals(humanPos, targetPos))
        {
            _tasks.Dequeue();
            if (_tasks.Count == 0)
                RefillTasks();
        }

        return _tasks.Peek();
    }

    private void RefillTasks()
    {
        /* for (int i = 0; i < 3; ++i) */
            _tasks.Enqueue(new WanderTask(World, Map));
            _tasks.Enqueue(new EnterHouseTask(World, Map));
    }
}

internal class Human : Movable
{
    public House House { get; private set; }
    public bool Homeless => House is null;

    public Workplace Job { get; private set; }
    public bool Jobless => Job is null;

    public Building OccupiedBuilding { get; private set; }

    public bool IsOutside => OccupiedBuilding is null;

    private Plan Plan { get; }

    public Human(Plan plan, RaycasterFactory raycasterFactory, PathBuilderFactory pathBuilderFactory, int id) : base(raycasterFactory, pathBuilderFactory, id, new Size(0.005, 0.005))
    {
        Plan = plan;

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

    public bool Enter(Building building)
    {
        if (OccupiedBuilding is not null)
            return false;

        OccupiedBuilding = building;
        return building.AddOccupant(this);
    }

    public bool Leave()
    {
        if (OccupiedBuilding is null)
            return false;

        var occupiedBuilding = OccupiedBuilding;
        OccupiedBuilding = null;
        return occupiedBuilding.RemoveOccupant(this);
    }

    protected override Point GetNewTarget()
    {
        var task = Plan.GetTask(this);
        return task.GetTargetPoint(this);
    }
}
