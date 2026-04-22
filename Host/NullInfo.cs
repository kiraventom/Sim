namespace Sim.Host;

internal readonly struct NullInfo() : IObjectInfo
{
    public string Text { get; } = "NULL";
}


