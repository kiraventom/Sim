using System;
using System.Collections.Generic;

namespace Sim.Model;

internal class Semaphore
{
    private readonly Dictionary<object, SemaphoreObject> _dict = [];

    public bool IsSet(object obj) => _dict.ContainsKey(obj);

    public SemaphoreObject SetOnce(object obj)
    {
        if (!_dict.TryGetValue(obj, out var state))
        {
            state = new SemaphoreObject();
            _dict[obj] = state;
        }

        return state;
    }
}

internal class SemaphoreObject : IDisposable
{
    public void Dispose()
    {
    }
}
