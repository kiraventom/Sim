using System.Collections.Generic;
using Sim.Utils;

namespace Sim.Host;

internal class IdContainer
{
    private readonly List<int> _ids = [];
    private readonly HashSet<int> _idsSet = [];

    public IReadOnlyList<int> Ids => _ids;

    public int NewId()
    {
        int id;

        do
        {
            id = RND.Int(1, int.MaxValue);
        }
        while (_idsSet.Contains(id));

        _ids.Add(id);
        _idsSet.Add(id);

        return id;
    }

    public void DropId(int id)
    {
        _ids.Remove(id);
        _idsSet.Remove(id);
    }
}

