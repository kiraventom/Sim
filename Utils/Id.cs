using System.Collections.Generic;

namespace Sim.Utils;

public static class Id
{
    public static int Generate<T>(IReadOnlyDictionary<int, T> collection)
    {
        int id;
        do
        {
            id = RND.Int(1, int.MaxValue);
        }
        while (collection.ContainsKey(id));

        return id;
    }
}
