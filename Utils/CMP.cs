using System;
using Sim.Geometry;

namespace Sim.Utils;

public static class CMP
{
    public const double DEFAULT_THRESHOLD = 1e-9;

    public static bool Equals(double a, double b, double threshold = DEFAULT_THRESHOLD)
    {
        return Math.Abs(a - b) < threshold;
    }

    public static bool Equals(Point a, Point b, double threshold = DEFAULT_THRESHOLD)
    {
        return Math.Abs(a.X - b.X) < threshold && Math.Abs(a.Y - b.Y) < threshold;
    }
}

