using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;
using Sim.Model;
using Positions = System.Collections.Generic.IReadOnlyDictionary<int, Sim.Geometry.PointI>;
using Sim.Geometry;

namespace Sim.Host;

public class WorldHost(ILogger<WorldHost> logger, World world, PositionsCache cache, WorldSettings settings) : BackgroundService
{
    private const int INTERVAL = 50;

    private bool _isPaused = false;

    public Size WorldSize => new Size(settings.MapWidth, settings.MapHeight);

    public void TogglePause()
    {
        _isPaused = !_isPaused;
    }

    public Positions GetPositions() => cache.GetPositions();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (!_isPaused)
                world.Tick();

            await Task.Delay(INTERVAL, stoppingToken);
        }
    }
}

