using System.Linq;
using Microsoft.Extensions.Logging;
using Positions = System.Collections.Generic.IReadOnlyDictionary<int, Sim.Geometry.PointI>;

namespace Sim.Model;

internal class PositionsCache(ILogger<PositionsCache> logger, Map map)
{
    private Positions _latest;

    public Positions GetPositions() => _latest;

    public void UpdateCache()
    {
        _latest = map.Positions.ToDictionary();
    }
}
