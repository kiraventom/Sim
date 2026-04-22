using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Sim.Geometry;
using Sim.Host;
using Sim.Model.Objects;

namespace Sim.Model.Entities;

internal class EntityBuilder(ILogger<EntityBuilder> logger, WorldSettings settings, World world, Map map)
{
    public IReadOnlyCollection<IEntity> BuildEntities()
    {
        List<IEntity> entities = [];
        foreach (var (id, point) in map.Positions)
        {
            if (!world.Objects.TryGetValue(id, out var obj))
            {
                logger.LogError("Object {Id} is present on map, but not found in world", id);
                continue;
            }

            var entityPoint = ToEntityPoint(point);

            IReadOnlyCollection<IEntity> entitiesToAdd = obj switch
            {
                Human h when h.Plans.FirstOrDefault() is Plan p => 
                [ 
                    new HumanEntity(new SizeI(1, 1), entityPoint), 
                    new LineEntity(entityPoint, ToEntityPoint(p.Target)) 
                ],
                Human h => 
                [ 
                    new HumanEntity(new SizeI(1, 1), entityPoint), 
                ],
            };

            entities.AddRange(entitiesToAdd);
        }

        entities.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        return entities;
    }

    private PointI ToEntityPoint(Point point) => (point * new Point(settings.MapWidth, settings.MapHeight)).ToPointI();
}

