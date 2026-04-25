using Sim.Model;
using Sim.Model.Entities;
using Sim.Model.Objects;

namespace Sim.Host;

internal class ObjectInfoBuilder(Map map, WorldSettings settings)
{
    public string Build(SimObject obj)
    {
        if (obj is null)
            return new NullInfo().Text;

        var rect = map.Rects[obj.Id].ToAbsRect(settings);

        return obj switch
        {
            Human h => new HumanInfo(h.Id, rect.Pos, h.Speed * settings.MapWidth, h.Movement.Start.ToAbsPoint(settings), h.Movement.End.ToAbsPoint(settings)).Text,
            _ => new DefaultInfo(obj.Id).Text
        };
    }
}


