using Sim.Geometry;

namespace Sim.Model.Objects;

internal abstract class Movable(int id) : SimObject(id)
{
    public const int ATTEMPTS_COUNT = 10;

    public double Speed { get; protected init; }
    
    private Point _targetPos = Point.INVALID;

    public bool Move(Map map)
    {
        for (int attempt = 0; ; ++attempt)
        {
            var offset = GetMoveOffset(map, forceNew: attempt != 0);
            if (offset.IsZero())
                break;

            if (map.TryMove(Id, offset))
                break;

            if (attempt == ATTEMPTS_COUNT)
                return false;
        }

        return true;
    }

    protected Point GetMoveOffset(Map map, bool forceNew = false)
    {
        if (forceNew)
            _targetPos = Point.INVALID;

        var pos = map[Id].Pos;

        if (HasReachedTarget(pos))
            _targetPos = GetNewTarget(map, pos);

        var traj = _targetPos - pos;
        var direction = traj.Normalize();

        var offset = (direction * Speed);

        if (offset.Length >= traj.Length)
            offset = traj;

        return offset;
    }

    protected abstract Point GetNewTarget(Map map, Point currentPos);

    protected bool HasReachedTarget(Point currentPos) => _targetPos.IsInvalid() || _targetPos == currentPos;
}

