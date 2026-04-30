using Sim.Geometry;

namespace Sim.Model.Objects;

internal abstract class Movable(PathBuilder pathBuilder, int id) : SimObject(id)
{
    public double Speed { get; protected init; }

    public Path Path { get; } = new();

    public Point GetMoveOffset(Point pos)
    {
        if (HasReachedTarget(pos))
        {
            Path.OnTargetReached();

            if (Path.IsCovered)
            {
                var target = GetNewTarget(pos);
                Path.New(pos, target);
                pathBuilder.BuildPath(this);
            }
        }

        var targetPos = Path.TargetPoint;

        var traj = targetPos - pos;
        var direction = traj.Normalize();

        var offset = (direction * Speed);

        if (offset.Length >= traj.Length)
            offset = traj;

        return offset;
    }

    protected abstract Point GetNewTarget(Point pos);

    protected bool HasReachedTarget(Point currentPos) => Path.IsCovered || new Rect(currentPos, new Size(0.0001, 0.0001)).Intersects(new Rect(Path.TargetPoint, new Size(0.0001, 0.0001)));
}

