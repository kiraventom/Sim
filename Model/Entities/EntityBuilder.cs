using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Sim.Geometry;
using Sim.Host;
using Sim.Model.Objects;

namespace Sim.Model.Entities;

internal class EntityBuilder(ILogger<EntityBuilder> logger, WorldSettings settings, World world, Map map)
{
    public IReadOnlyList<IEntity> BuildEntities()
    {
        List<IEntity> entities = [];
        foreach (var (id, point) in map.Positions)
        {
            if (!world.Objects.TryGetValue(id, out var obj))
            {
                logger.LogError("Object {Id} is present on map, but not found in world", id);
                continue;
            }

            var absPoint = point.ToAbsPoint(settings);

            IReadOnlyCollection<IEntity> entitiesToAdd = obj switch
            {
                Human h when h.Plans.First() is Plan p => 
                [ 
                    new HumanEntity(h.Id, new SizeI(1, 1), absPoint),
                    new LineEntity(id, absPoint, p.Target.ToAbsPoint(settings)) 
                ],
            };

            entities.AddRange(entitiesToAdd);
        }

        entities.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        return entities;
    }
}
