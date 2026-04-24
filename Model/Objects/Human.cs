using System.Collections.Generic;
using Sim.Geometry;
using Sim.Utils;

namespace Sim.Model.Objects;

internal class Human : Movable
{
    private readonly Queue<Plan> _plans = [];

    public IReadOnlyCollection<Plan> Plans => _plans;

    public override Size Size => new Size(0.005, 0.005);

    public Human(int id) : base(id)
    {
        const double SpeedModMin = 0.0015;
        const double SpeedModMax = 0.003;
        Speed = RND.Double(SpeedModMin, SpeedModMax);
    }

    protected override Point GetNewTarget(Map map, Point currentPos)
    {
        _plans.TryDequeue(out _);

        if (_plans.Count == 0)
            _plans.Enqueue(new Plan(currentPos, RND.Point(1, 1)));

        return _plans.Peek().Target;
    }
}
