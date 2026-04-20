using Sim.Geometry;
using System;

namespace Sim.Utils;

public static class RND
{
    private static Random _rnd;

    static RND()
    {
        _rnd = new Random();
    }

    public static bool Bool() => _rnd.Next(2) == 1;
    public static int Int() => _rnd.Next();
    public static int Int(int max) => _rnd.Next(max);
    public static int Int(int min, int max) => _rnd.Next(min, max);
    public static double Double() => _rnd.NextDouble();
    public static double Double(double max) => _rnd.NextDouble() * max;
    public static double Double(double min, double max) => min + (_rnd.NextDouble() * (max - min));
    public static PointI PointI(Size s) => PointI(s.Width, s.Height);
    public static PointI PointI(int width, int height) => new PointI(_rnd.Next(width), _rnd.Next(height));
}
