using System.Collections.Generic;

namespace Sim.Model.Entities;

public class EntitySnapshot
{
    private readonly List<AreaEntity> _areas = new();
    private readonly List<ObstacleEntity> _obstacles = new();
    private readonly List<PathPartEntity> _pathParts = new();
    private readonly List<HumanEntity> _humans = new();
    private readonly List<BuildingEntity> _buildings = new();

    public IReadOnlyList<AreaEntity> Areas => _areas;
    public IReadOnlyList<ObstacleEntity> Obstacles => _obstacles;
    public IReadOnlyList<PathPartEntity> PathParts => _pathParts;
    public IReadOnlyList<HumanEntity> Humans => _humans;
    public IReadOnlyList<BuildingEntity> Buildings => _buildings;

    internal void Add(ObstacleEntity obstacle) => _obstacles.Add(obstacle);
    internal void Add(AreaEntity area) => _areas.Add(area);
    internal void Add(PathPartEntity line) => _pathParts.Add(line);
    internal void Add(HumanEntity human) => _humans.Add(human);
    internal void Add(BuildingEntity building) => _buildings.Add(building);

    internal void Clear()
    {
        _areas.Clear();
        _obstacles.Clear();
        _pathParts.Clear();
        _humans.Clear();
        _buildings.Clear();
    }

    internal void CloneFrom(EntitySnapshot source)
    {
        Clear();

        _areas.AddRange(source.Areas);
        _obstacles.AddRange(source.Obstacles);
        _pathParts.AddRange(source.PathParts);
        _humans.AddRange(source.Humans);
        _buildings.AddRange(source.Buildings);
    }
}
