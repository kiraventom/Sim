using Sim.Geometry;

namespace Sim.Host;

internal readonly struct HumanInfo(int id, RectI rect, double speed, PointI initialPos, PointI targetPos) : IObjectInfo
{
    public string Text { get; } =
    $"""
    Id : {id}
    TopLeft: {rect.TopLeft}
    Size: {rect.Size}
    Speed: {speed}
    From: {initialPos}
    To: {targetPos}
    """;
}

    // Path: [ {string.Join(" -> ", path)} ]


