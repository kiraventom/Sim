using Sim.Geometry;

namespace Sim.Model.Objects;

internal abstract class Movable(Pathfinder pathfinder, int id) : SimObject(id)
{
    public double Speed { get; protected init; }

    public Movement Movement { get; } = new();

    public Point GetMoveOffset(Point pos)
    {
        if (HasReachedTarget(pos))
        {
            if (Movement.Points.Count != 0)
                Movement.Points.RemoveFirst();

            if (Movement.Points.Count == 0)
            {
                var target = GetNewTarget(pos);
                Movement.Populate(pos, target);
            }
        }

        pathfinder.AdjustMovement(this);

        var targetPos = Movement.GetTarget();

        var traj = targetPos - pos;
        var direction = traj.Normalize();

        var offset = (direction * Speed);

        if (offset.Length >= traj.Length)
            offset = traj;

        return offset;
    }

    protected abstract Point GetNewTarget(Point pos);

    protected bool HasReachedTarget(Point currentPos) => Movement.Points.Count == 0 || new Rect(currentPos, new Size(0.0001, 0.0001)).Intersects(new Rect(Movement.GetTarget(), new Size(0.0001, 0.0001)));
}

