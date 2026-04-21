using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;
using Sim.Model;
using Sim.Geometry;
using System.Collections.Generic;

namespace Sim.Host;

internal class WorldHost : BackgroundService, IWorldHost
{
    private const int INTERVAL = 50;
    private readonly World _world;
    private readonly ObjectsCache _cache;
    private readonly WorldSettings _settings;
    private bool _isPaused = false;

    public WorldHost(ILogger<WorldHost> logger, World world, ObjectsCache cache, WorldSettings settings)
    {
        _world = world;
        _cache = cache;
        _settings = settings;
    }

    public SizeI WorldSize => new SizeI(_settings.MapWidth, _settings.MapHeight);

    public void TogglePause()
    {
        _isPaused = !_isPaused;
    }

    public IReadOnlyCollection<IObject> GetObjects() => _cache.GetObjects();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (!_isPaused)
                _world.Tick();

            await Task.Delay(INTERVAL, stoppingToken);
        }
    }
}

