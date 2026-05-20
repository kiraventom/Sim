using System.Collections.Generic;
using Sim.Geometry;

namespace Sim.Host;

internal readonly struct HumanInfo(int id, RectI rect, double speed, PointI initialPos, PointI targetPos, IReadOnlyList<int> detectedIds) : IObjectInfo
{
    public string Text { get; } =
    $"""
    Id : {id}
    TopLeft: {rect.TopLeft}
    Size: {rect.Size}
    Speed: {speed}
    From: {initialPos}
    To: {targetPos}
    Detected: [{string.Join(", ", detectedIds)}]
    """;
}

    // Path: [ {string.Join(" -> ", path)} ]


