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
        int hash = 0;
        foreach (var id in ids)
        {
            hash += id.GetHashCode();
        }

        return hash;
    }
}

