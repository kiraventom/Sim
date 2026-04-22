namespace Sim.Host;

internal readonly struct DefaultInfo(int id) : IObjectInfo
{
    public string Text { get; } = $"Id: {id}";
}


