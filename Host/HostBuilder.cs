using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.IO;
using static System.Environment;
using Sim.Model;
using Sim.View;

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
    }

    public static HostBuilder Create() => new HostBuilder();

    public HostBuilder Settings(WorldSettings s)
    {
        _settings = s;
        return this;
    }

    public HostBuilder ConfigureLogging(Action<string> logAction)
    {
        _builder.Services.AddSerilog((sp, l) =>
        {
            var paths = sp.GetRequiredService<Paths>();
            var logsDirPath = Path.Combine(paths.DataDir, "logs");
            Directory.CreateDirectory(logsDirPath);
            var logFilePath = Path.Combine(logsDirPath, $"{PROJECT_NAME}.log");

            l.MinimumLevel.Debug()
                .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
                .WriteTo.Observers(events => events.Subscribe(new UIObserver(logAction)));
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
            .AddSingleton<PositionsCache>()
            .AddSingleton<Map>()
            .AddSingleton<World>()
            .AddHostedService<WorldHost>();

        return _builder.Build();
    }
}

