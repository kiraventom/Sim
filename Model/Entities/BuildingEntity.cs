using Sim.Geometry;
using Sim.Host;

namespace Sim.Model.Entities;

public readonly struct BuildingEntity(int id, RectI rect, BuildingType type) : IRectEntity
{
    public int ObjectId => id;
    public RectI Rect => rect;
    public BuildingType Type => type;
}

