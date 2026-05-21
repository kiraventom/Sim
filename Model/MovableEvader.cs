using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Sim.Model;

// TODO Move static pathbuilding here
// TODO Then rename class
internal class MovableEvader(ILogger<MovableEvader> logger, MovableDetector detector)
{
    private readonly Dictionary<int, MovableAction> _actions = [];
    private readonly Dictionary<int, Intersection> _intersections = [];

    public event Action<int> PathBuilt;
    public event Action<int> Frozen;

    public MovableAction? GetCurrentAction(int movableId) => _actions.TryGetValue(movableId, out var action) ? action : null;

    public MovableAction GetNewAction(int movableId)
    {
        UpdateActions(movableId);
        return _actions[movableId];
    }

    public void NotifyPathBuilt(int id)
    {
        if (_actions[id] == MovableAction.BuildPath)
        {
            _actions[id] = MovableAction.Continue;
            PathBuilt?.Invoke(id);
        }
    }

    private void UpdateActions(int movableId)
    {
        var detectedIds = detector.Detect(movableId).ToList();

        var hash = Intersection.CalcHash(movableId, detectedIds);
        if (_intersections.TryGetValue(movableId, out var intersection))
        {
            if (intersection.Hash == hash)
                return;

            foreach (var id in intersection.Ids)
            {
                _intersections.Remove(id);
                _actions.Remove(id);
            }
        }

        if (detectedIds.Count == 0)
        {
            _actions[movableId] = MovableAction.Continue;
            return;
        }

        var newIntersection = new Intersection(movableId, detectedIds);
        foreach (var id in newIntersection.Ids)
        {
            if (_intersections.TryGetValue(id, out var oldIntersection))
            {
                foreach (var oldInterId in oldIntersection.Ids)
                {
                    _intersections.Remove(oldInterId);
                    _actions.Remove(oldInterId);
                }
            }

            _intersections.Add(id, newIntersection);
        }

        int maxDetectedId = newIntersection.Ids.Max();

        foreach (var id in newIntersection.Ids)
        {
            var action = id switch
            {
                int i when i == maxDetectedId => MovableAction.BuildPath,
                int i when i < maxDetectedId => MovableAction.Freeze,
            };

            if (action == MovableAction.Freeze)
                Frozen?.Invoke(id);

            _actions[id] = action;
        }

        return;
    }
}

