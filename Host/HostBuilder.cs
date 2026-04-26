using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.IO;
using static System.Environment;
using Sim.Model;
using Sim.Model.Entities;

namespace Sim.Host;

public class HostBuilder
{
    private const string PROJECT_NAME = "Sim";

    private HostApplicationBuilder _builder;
    private WorldSettings _settings = new WorldSettings();

    private HostBuilder()
    {
        _builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder();
        _builder.Services.AddSingleton<Paths>(BuildPaths);

        ConfigureLogging();
    }

    public static HostBuilder Create() => new HostBuilder();

    public HostBuilder UseCommandLineArgs()
    {
        var args = Environment.GetCommandLineArgs();
        int humans = _settings.HumansCount;
        int obstacles = _settings.ObstaclesCount;
        int width = _settings.MapWidth;
        int height = _settings.MapHeight;

        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == "--humans" && int.TryParse(args[i + 1], out int h)) 
                humans = h;
            else if (args[i] == "--obstacles" && int.TryParse(args[i + 1], out int o)) 
                obstacles = o;
            else if (args[i] == "--width" && int.TryParse(args[i + 1], out int w)) 
                width = w;
            else if (args[i] == "--height" && int.TryParse(args[i + 1], out int ht)) 
                height = ht;
        }

        _settings = new WorldSettings(humans, obstacles, width, height);
        return this;
    }

    private HostBuilder ConfigureLogging()
    {
        _builder.Services.AddSerilog((sp, l) =>
        {
            var paths = sp.GetRequiredService<Paths>();
            var logsDirPath = Path.Combine(paths.DataDir, "logs");
            Directory.CreateDirectory(logsDirPath);
            var logFilePath = Path.Combine(logsDirPath, $"{PROJECT_NAME}.log");

            l.MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day);
        });

        return this;
    }

    private static Paths BuildPaths(IServiceProvider provider)
    {
        var appData = Environment.GetFolderPath(SpecialFolder.ApplicationData);
        var localAppData = Environment.GetFolderPath(SpecialFolder.LocalApplicationData);

        var configDirPath = Path.Combine(appData, PROJECT_NAME);
        var dataDirPath = Path.Combine(localAppData, PROJECT_NAME);

        Directory.CreateDirectory(configDirPath);
        Directory.CreateDirectory(dataDirPath);

        return new Paths(configDirPath, dataDirPath);
    }

    public IHost Build()
    {
        _builder.Services
            .AddSingleton<WorldSettings>(_settings)
            .AddSingleton<Map>()
            .AddSingleton<World>()
            .AddSingleton<ObjectInfoBuilder>()
            .AddSingleton<Pathfinder>()
            .AddSingleton<EntityBuilder>()
            .AddSingleton<EntityCache>()
            .AddSingleton<IdContainer>()
            .AddSingleton<HumanFactory>()
            .AddSingleton<ObstacleFactory>()
            .AddHostedService<WorldHost>();

        var host = _builder.Build();

        Log.Logger = host.Services.GetRequiredService<ILogger>();
        return host;
    }
}

