using Sim.Geometry;
using Sim.Utils;

namespace Sim.Model.Objects;

internal abstract class Movable(PathBuilderFactory pathBuilderFactory, int id) : SimObject(id)
{
    public double Speed { get; protected init; }

    public Path Path { get; private set; }

    public Point GetMoveOffset(Point pos)
    {
        UpdatePath(pos);

        return GetDirectMoveOffset(pos, Path.TargetPoint);
    }

    internal Point GetDirectMoveOffset(Point pos, Point targetPos)
    {
        var traj = targetPos - pos;
        var direction = traj.Normalize();

        var offset = (direction * Speed);

        if (offset.Length >= traj.Length)
            offset = traj;

        return offset;
    }

    protected abstract Point GetNewTarget(Point pos);

    private void UpdatePath(Point pos)
    {
        Path?.UpdateTarget(pos);

        if (Path is { IsCovered: false })
            return;

        Path = null;

        var pathBuilder = pathBuilderFactory.Build(Id, Size);

        // TODO: More complex logic in case of failing to build path
        while (Path is null)
        {
            var target = GetNewTarget(pos);
            var pathBuilt = pathBuilder.TryBuildPath(pos, target, out var path);
            if (pathBuilt)
                Path = path;
        }
    }
}
