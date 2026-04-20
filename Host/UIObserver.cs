using Serilog.Events;
using System;

namespace Sim.Host;

public class UIObserver : IObserver<LogEvent>
{
    private Action<string> _logAction;
    public UIObserver(Action<string> logAction) => _logAction = logAction;
    public void OnNext(LogEvent value) => Avalonia.Threading.Dispatcher.UIThread.Post(() => _logAction(value.RenderMessage()));
    public void OnError(Exception error) {}
    public void OnCompleted() {}
}

