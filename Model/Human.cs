using Sim.Geometry;
using Sim.Utils;

namespace Sim.Model;

internal class Human
{
    public int Id { get; }

    private const double SpeedModMin = 0.00125;
    private const double SpeedModMax = 0.00625;

    public double Speed { get; }
    
    private PointI _targetPos = PointI.INVALID;

    public Human(int id)
    {
        Id = id;
        Speed = RND.Double(SpeedModMin, SpeedModMax);
    }

    public PointI GetMoveOffset(Map map, bool forceNew = false)
    {
        if (forceNew)
            _targetPos = PointI.INVALID;

        var pos = map[Id];
        _targetPos = GetTargetPos(map, pos);

        var traj = _targetPos - pos;
        var direction = traj.Normalize();
        var speedVec = map.Size.ToPointI() * Speed;

        var offset = (direction * speedVec).ToPointI();
        if (offset.Length < 1)
            offset = direction.ToPointI();

        if (offset.Length >= traj.Length)
            offset = traj;

        return offset;
    }

    private PointI GetTargetPos(Map map, PointI currentPos)
    {
        if (_targetPos == PointI.INVALID || _targetPos == currentPos)
        {
            return RND.PointI(map.Size);
        }

        return _targetPos;
    }
}
