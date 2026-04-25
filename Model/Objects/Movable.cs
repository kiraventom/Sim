using Sim.Geometry;

namespace Sim.Model.Objects;

internal abstract class Movable(Pathfinder pathfinder, int id) : SimObject(id)
{
    public double Speed { get; protected init; }

    public Movement Movement { get; private set; }

    public Point GetMoveOffset(Point pos)
    {
        if (HasReachedTarget(pos))
        {
            var target = GetNewTarget(pos);
            Movement = new Movement(pos, target);
        }

        pathfinder.CorrectMovement(this, pos);

        var targetPos = Movement.GetTarget();

        var traj = targetPos - pos;
        var direction = traj.Normalize();

        var offset = (direction * Speed);

        if (offset.Length >= traj.Length)
            offset = traj;

        return offset;
    }

    protected abstract Point GetNewTarget(Point pos);

    protected bool HasReachedTarget(Point currentPos) => Movement is null || currentPos == Movement.End;
}

