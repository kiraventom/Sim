using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;
using Sim.Model;

namespace Sim.Host;

public class WorldHost(ILogger<WorldHost> logger, World world) : BackgroundService
{
    private const int INTERVAL = 50;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            world.Tick();
            await Task.Delay(INTERVAL, stoppingToken);
        }
    }
}

