using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;
using Sim.Model;
using Sim.Geometry;
using System.Collections.Generic;
using Sim.Model.Entities;
using System;
using Sim.Model.Objects;

namespace Sim.Host;

internal class WorldHost(ILogger<WorldHost> logger, World world, Map map, IdContainer idContainer, EntityCache cache, ObjectInfoBuilder infoBuilder, WorldSettings settings, HumanFactory humanFactory, ObstacleFactory obstacleFactory) : BackgroundService, IWorldHost
{
    private const int INTERVAL = (int)(1000.0 / 60);

    private bool _isPaused = false;
    private int _selectedIndex = -1;

    public SizeI WorldSize => new SizeI(settings.MapWidth, settings.MapHeight);

    public int SelectedObjectId => _selectedIndex >= 0 ? idContainer.Ids[_selectedIndex] : -1;

    public void UpdateSnapshot(EntitySnapshot snapshot) => cache.UpdateSnapshot(snapshot);

    public void TogglePause() => _isPaused = !_isPaused;

    public void SelectNextObject()
    {
        if (_selectedIndex == idContainer.Ids.Count - 1)
            _selectedIndex = 0;
        else
            _selectedIndex++;
    }

    public void SelectPrevObject()
    {
        if (_selectedIndex == 0)
            _selectedIndex = idContainer.Ids.Count - 1;
        else
            _selectedIndex--;
    }

    public string GetInfo(int id) => infoBuilder.Build(world.Objects.GetValueOrDefault(id));

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        PlaceObjects();

        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(INTERVAL));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            if (!_isPaused)
                world.Tick();
        }
    }

    private void PlaceObjects()
    {
        var objs = GenerateObjects();

        foreach (var obj in objs)
        {
            var rect = map.RandomFreeRect(obj.Size);
            if (rect.IsInvalid())
            {
                logger.LogError("Failed to find free rect for size {Size}, skipping", obj.Size);
                continue;
            }

            if (map.TryPlace(obj.Id, rect))
            {
                world.AddObject(obj);
                logger.LogTrace("Placed {Id} at {Rect}", obj.Id, rect);
            }
            else
            {
                logger.LogError("Failed to place {Id} at {Rect}, skipping", obj.Id, rect);
            }
        }
    }

    private IEnumerable<SimObject> GenerateObjects()
    {
        for (int i = 0; i < settings.ObstaclesCount; ++i)
            yield return obstacleFactory.Build();

        for (int i = 0; i < settings.HumansCount; ++i)
            yield return humanFactory.Build();
    }

    public void UnselectObject()
    {
        _selectedIndex = -1;
    }
}
