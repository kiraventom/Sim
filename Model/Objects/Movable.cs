using Sim.Geometry;
using Sim.Utils;

namespace Sim.Model.Objects;

internal abstract class Movable(PathBuilderFactory pathBuilderFactory, int id) : SimObject(id)
{
    public double Speed { get; protected init; }

    public Path Path { get; private set; }

    internal Point GetDirectMoveOffset(Point pos, Point targetPos)
    {
        var traj = targetPos - pos;
        var direction = traj.Normalize();

        var offset = (direction * Speed);

        if (offset.Length >= traj.Length)
            offset = traj;

        return offset;
    }

    public Point GetMoveOffset(Point pos)
    {
        if (HasReachedTarget(pos))
        {
            Path?.OnTargetReached();

            if (Path is null || Path.IsCovered)
            {
                var target = GetNewTarget(pos);
                var pathBuilder = pathBuilderFactory.Build(Id, Size);
                Path = pathBuilder.BuildPath(pos, target);
            }
        }

        var targetPos = Path.TargetPoint;
        return GetDirectMoveOffset(pos, targetPos);
    }

    protected abstract Point GetNewTarget(Point pos);

    protected bool HasReachedTarget(Point currentPos) => Path is null || Path.IsCovered || CMP.Equals(currentPos, Path.TargetPoint);
}

