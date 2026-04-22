using Sim.Geometry;

namespace Sim.Host;

internal readonly struct HumanInfo(int id, PointI pos, double speed, PointI initialPos, PointI targetPos) : IObjectInfo
{
    public string Text { get; } =
    $"""
    Id : {id}
    Pos: {pos}
    Speed: {speed}
    From: {initialPos}
    To: {targetPos}
    """;
}


