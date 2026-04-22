using Sim.Model;
using Sim.Model.Entities;
using System.Linq;
using Sim.Model.Objects;

namespace Sim.Host;

internal class ObjectInfoBuilder(Map map, WorldSettings settings)
{
    public IObjectInfo Build(SimObject obj)
    {
        if (obj is null)
            return new NullInfo();

        var pos = map.Positions[obj.Id].ToAbsPoint(settings);

        return obj switch
        {
            Human h => new HumanInfo(h.Id, pos, h.Speed * settings.MapWidth, h.Plans.First().Start.ToAbsPoint(settings), h.Plans.First().Target.ToAbsPoint(settings)),
            _ => new DefaultInfo(obj.Id)
        };
    }
}


