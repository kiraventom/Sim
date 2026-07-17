using Sim.Geometry;
using Sim.Utils;

namespace Sim.Model.Objects;

internal abstract class Movable : SimObject
{
    private PathBuilder PathBuilder { get; }
    private RaycasterFactory RaycasterFactory { get; }

    public override bool HasCollision { get; } = false;

    public Movable(RaycasterFactory raycasterFactory, PathBuilderFactory pathBuilderFactory, int id, Size size) : base(id, size)
    {
        RaycasterFactory = raycasterFactory;
        PathBuilder = pathBuilderFactory.Build(Id, Size, raycasterFactory);
    }

    public double Speed { get; protected init; }

    public Path Path { get; private set; }

    public Point GetMoveOffset(Point pos)
    {
        UpdatePath(pos);
        if (Path is null)
            return Point.ZERO;

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

    protected abstract Point GetNewTarget();

    private void UpdatePath(Point pos)
    {
        Path?.UpdateTarget(pos);

        if (Path is { IsCovered: false })
            return;

        Path = null;

        while (Path is null)
        {
            var target = GetNewTarget();
            if (CMP.Equals(pos, target))
                return;

            var pathBuilt = PathBuilder.TryBuildPath(pos, target, out var path);
            if (pathBuilt)
                Path = path;
        }
    }
}
