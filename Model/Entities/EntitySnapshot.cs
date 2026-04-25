using System.Collections.Generic;

namespace Sim.Model.Entities;

public class EntitySnapshot
{
    private readonly List<AreaEntity> _areas = new();
    private readonly List<ObstacleEntity> _obstacles = new();
    private readonly List<LineEntity> _lines = new();
    private readonly List<HumanEntity> _humans = new();

    public IReadOnlyList<AreaEntity> Areas => _areas;
    public IReadOnlyList<ObstacleEntity> Obstacles => _obstacles;
    public IReadOnlyList<LineEntity> Lines => _lines;
    public IReadOnlyList<HumanEntity> Humans => _humans;

    internal void Add(ObstacleEntity obstacle) => _obstacles.Add(obstacle);
    internal void Add(AreaEntity area) => _areas.Add(area);
    internal void Add(LineEntity line) => _lines.Add(line);
    internal void Add(HumanEntity human) => _humans.Add(human);

    internal void Clear()
    {
        _areas.Clear();
        _obstacles.Clear();
        _lines.Clear();
        _humans.Clear();
    }

    internal void CloneFrom(EntitySnapshot source)
    {
        Clear();

        _areas.AddRange(source.Areas);
        _obstacles.AddRange(source.Obstacles);
        _lines.AddRange(source.Lines);
        _humans.AddRange(source.Humans);
    }
}
