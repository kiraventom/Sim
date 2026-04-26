using System.Collections.Generic;

namespace Sim.Model;

internal class Area
{
    private readonly List<int> _objectIds = [];
    public IReadOnlyList<int> ObjectIds => _objectIds;

    public void Add(int id) => _objectIds.Add(id);
    public void Remove(int id) => _objectIds.Remove(id);
}

