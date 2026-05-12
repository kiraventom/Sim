using Sim.Geometry;

namespace Sim.Model.Objects;

internal abstract class Movable : SimObject
{
    private MovableDetector Detector { get; }
    private PathBuilder PathBuilder { get; }
    private RaycasterFactory RaycasterFactory { get; }

    public bool IsFrozen { get; set; }

    public Movable(MovableDetector movableDetector, RaycasterFactory raycasterFactory, PathBuilderFactory pathBuilderFactory, int id) : base(id)
    {
        Detector = movableDetector;
        RaycasterFactory = raycasterFactory;
        PathBuilder = pathBuilderFactory.Build(Id, Size, raycasterFactory);
    }

    public double Speed { get; protected init; }

    public Path Path { get; private set; }

    public Point GetMoveOffset(Point pos)
    {
        UpdatePath(pos);
        var offset = GetDirectMoveOffset(pos, Path.TargetPoint);

        IsFrozen = false;

        var detectedIds = Detector.Detect(Id, offset);
        int? maxDetectedId = null;
        foreach (var detectedId in detectedIds)
        {
            if (maxDetectedId is null || detectedId > maxDetectedId)
                maxDetectedId = detectedId;
        }

        // if anything detected
        if (maxDetectedId != null)
        {
            if (Id > maxDetectedId.Value)
            {
                if (PathBuilder.TryBuildPath(pos, Path.End, out var path))
                {
                    Path = path;
                    return GetDirectMoveOffset(pos, Path.TargetPoint);
                }
            }
            else
            {
                IsFrozen = true;
                return Point.ZERO;
            }
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
