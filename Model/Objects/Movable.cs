using Sim.Geometry;

namespace Sim.Model.Objects;

internal abstract class Movable : SimObject
{
    private PathBuilder PathBuilder { get; }
    private RaycasterFactory RaycasterFactory { get; }

    public override bool HasCollision { get; } = false;

    public Movable(RaycasterFactory raycasterFactory, PathBuilderFactory pathBuilderFactory, int id) : base(id)
    {
        RaycasterFactory = raycasterFactory;
        PathBuilder = pathBuilderFactory.Build(Id, Size, raycasterFactory);
    }

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

        while (Path is null)
        {
            var target = GetNewTarget(pos);
            var pathBuilt = PathBuilder.TryBuildPath(pos, target, out var path);
            if (pathBuilt)
                Path = path;
        }
    }
}
