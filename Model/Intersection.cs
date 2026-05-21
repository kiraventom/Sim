using System;
using System.Collections.Generic;
using System.Linq;

namespace Sim.Model;

internal class Intersection
{
    public int Hash { get; }
    public IReadOnlyList<int> Ids { get; }

    public Intersection(int id, IReadOnlyList<int> ids)
    {
        Ids = [id, .. ids];
        Hash = CalcHash(Ids);
    }

    public static int CalcHash(int id, IEnumerable<int> ids) => CalcHash(ids.Prepend(id));

    public static int CalcHash(IEnumerable<int> ids)
    {
        var hash = new HashCode();
        foreach (var id in ids.OrderBy(x => x))
            hash.Add(id);
        return hash.ToHashCode();
    }
}

