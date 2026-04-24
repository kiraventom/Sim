using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Sim.Host;
using Sim.Model.Objects;

namespace Sim.Model.Entities;

internal class EntityBuilder(ILogger<EntityBuilder> logger, WorldSettings settings, World world, Map map)
{
    public IReadOnlyList<IEntity> BuildEntities()
    {
        List<IEntity> entities = [];
        foreach (var (id, rect) in map.Rects)
        {
            if (!world.Objects.TryGetValue(id, out var obj))
            {
                logger.LogError("Object {Id} is present on map, but not found in world", id);
                continue;
            }

            var absRect = rect.ToAbsRect(settings);

            IReadOnlyCollection<IEntity> entitiesToAdd = obj switch
            {
                Human h when h.Plans.First() is Plan p => 
                [ 
                    new HumanEntity(h.Id, absRect),
                    new LineEntity(id, absRect.Pos, p.Target.ToAbsPoint(settings)) 
                ],
                Obstacle o => 
                [
                    new ObstacleEntity(o.Id, absRect)
                ]
            };

            entities.AddRange(entitiesToAdd);
        }

        entities.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        return entities;
    }
}
