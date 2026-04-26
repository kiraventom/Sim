using System.Collections.Generic;

namespace Sim.Geometry;

public class RaycastResult
{
    private readonly List<RaycastHit> _hits = [];
    private readonly HashSet<int> _ids = [];

    public IReadOnlyList<RaycastHit> Hits => _hits;

    public static RaycastResult NoHits { get; } = new RaycastResult();

    internal void Add(RaycastHit hit)
    {
        _hits.Add(hit);
        _ids.Add(hit.Id);
    }

    internal bool ContainsId(int id) => _ids.Contains(id);
}

