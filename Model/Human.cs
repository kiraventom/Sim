using System;
using Sim.Geometry;
using Sim.Utils;

namespace Sim.Model;

public class Human(int id)
{
    public int Id { get; } = id;
    public double Speed { get; } = RND.Int(1, 5);
    
    private PointI _targetPos = PointI.INVALID;

    public PointI GetMoveOffset(Map map, bool forceNew = false)
    {
        if (forceNew)
            _targetPos = PointI.INVALID;

        var pos = map[Id];
        _targetPos = GetTargetPos(pos);
        var traj = _targetPos - pos;
        var direction = traj.Normalize();
        var offset = (direction * Speed).ToPointI();
        if (offset.Length < 1)
            offset = direction.ToPointI();

        if (offset.Length >= traj.Length)
            offset = traj;

        return offset;
    }

    private PointI GetTargetPos(PointI currentPos)
    {
        if (_targetPos == PointI.INVALID || _targetPos == currentPos)
        {
            var rel = RND.PointI(200, 200) - new PointI(100, 100);
            return currentPos + rel;
        }

        return _targetPos;
    }
}
