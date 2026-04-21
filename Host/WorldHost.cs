using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;
using Sim.Model;
using Positions = System.Collections.Generic.IReadOnlyDictionary<int, Sim.Geometry.PointI>;
using Sim.Geometry;

namespace Sim.Host;

internal class WorldHost : BackgroundService, IWorldHost
{
    private const int INTERVAL = 50;
    private readonly World _world;
    private readonly PositionsCache _cache;
    private readonly WorldSettings _settings;
    private bool _isPaused = false;

    public WorldHost(ILogger<WorldHost> logger, World world, PositionsCache cache, WorldSettings settings)
    {
        _world = world;
        _cache = cache;
        _settings = settings;
    }

    public Size WorldSize => new Size(_settings.MapWidth, _settings.MapHeight);

    public void TogglePause()
    {
        _isPaused = !_isPaused;
    }

    public Positions GetPositions() => _cache.GetPositions();

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

