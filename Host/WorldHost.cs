using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;
using Sim.Model;
using Sim.Geometry;
using System.Collections.Generic;
using Sim.Model.Entities;
using System.Linq;
using System;

namespace Sim.Host;

internal class WorldHost : BackgroundService, IWorldHost
{
    private const int INTERVAL = (int)(1000.0 / 60);
    private readonly World _world;
    private readonly EntityCache _cache;
    private readonly ObjectInfoBuilder _infoBuilder;
    private readonly WorldSettings _settings;

    private bool _isPaused = false;
    private int _selectedIndex = -1;

    private List<int> ObjectIds { get; }

    public SizeI WorldSize => new SizeI(_settings.MapWidth, _settings.MapHeight);

    public WorldHost(ILogger<WorldHost> logger, World world, EntityCache cache, ObjectInfoBuilder infoBuilder, WorldSettings settings)
    {
        _world = world;
        _cache = cache;
        _infoBuilder = infoBuilder;
        _settings = settings;

        ObjectIds = world.Objects.Keys.ToList();
    }

    public void TogglePause()
    {
        _isPaused = !_isPaused;
    }

    public int SelectedObjectId => _selectedIndex >= 0 ? ObjectIds[_selectedIndex] : -1;

    public void UpdateSnapshot(EntitySnapshot snapshot) => _cache.UpdateSnapshot(snapshot);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(INTERVAL));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            if (!_isPaused)
                _world.Tick();
        }
    }

    public void SelectNextObject()
    {
        if (_selectedIndex == ObjectIds.Count - 1)
            _selectedIndex = 0;
        else
            _selectedIndex++;
    }

    public void SelectPrevObject()
    {
        if (_selectedIndex == 0)
            _selectedIndex = ObjectIds.Count - 1;
        else
            _selectedIndex--;
    }

    public bool SelectObject(int id)
    {
        _selectedIndex = ObjectIds.IndexOf(id);
        return _selectedIndex >= 0;
    }

    public string GetInfo(int id) => _infoBuilder.Build(_world.Objects.GetValueOrDefault(id));
}
