using Sim.Geometry;

namespace Sim.Model.Objects;

internal abstract class Movable : SimObject
{
    private MovableEvader Evader { get; }
    private PathBuilder PathBuilder { get; }
    private RaycasterFactory RaycasterFactory { get; }

    public Movable(MovableEvader movableEvader, RaycasterFactory raycasterFactory, PathBuilderFactory pathBuilderFactory, int id) : base(id)
    {
        Evader = movableEvader;
        RaycasterFactory = raycasterFactory;
        PathBuilder = pathBuilderFactory.Build(Id, Size, raycasterFactory);
    }

    public double Speed { get; protected init; }

    public Path Path { get; private set; }

    public Point GetMoveOffset(Point pos)
    {
        UpdatePath(pos);
        var offset = GetDirectMoveOffset(pos, Path.TargetPoint);

        var action = Evader.GetNewAction(Id);
        switch (action)
        {
            case MovableAction.BuildPath:
                if (PathBuilder.TryBuildPath(pos, Path.End, out var path))
                {
                    Path = path;
                    Evader.NotifyPathBuilt(Id);
                    return GetDirectMoveOffset(pos, Path.TargetPoint);
                }
                break;

            case MovableAction.Freeze:
                return Point.ZERO;

            case MovableAction.Continue:
                return offset;
        }

        return offset;
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

        // TODO: More complex logic in case of failing to build path
        while (Path is null)
        {
            var target = GetNewTarget(pos);
            var pathBuilt = PathBuilder.TryBuildPath(pos, target, out var path);
            if (pathBuilt)
                Path = path;
        }
    }
}
