using Sim.Geometry;
using Sim.Model.Objects;

namespace Sim.Model;

internal class Pathfinder(Map map)
{
    public void CorrectMovement(Movable movable, Point currentPos)
    {
        var target = movable.Movement.GetTarget();
        if (currentPos == target)
            movable.Movement.Points.RemoveAt(0);
    }
}

