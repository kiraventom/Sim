using Sim.Geometry;
using System;

namespace Sim.Utils;

public static class RND
{
    private static Random _rnd;

    public static int Seed { get; private set; }

    static RND()
    {
        var randomSeed = Random.Shared.Next();
        _rnd = new Random(randomSeed);
        Seed = randomSeed;
    }

    public static void SetSeed(int seed)
    {
        _rnd = new Random(seed);
        Seed = seed;
    }

    public static bool Bool() => _rnd.Next(2) == 1;
    public static int Int() => _rnd.Next();
    public static int Int(int max) => _rnd.Next(max);
    public static int Int(int min, int max) => _rnd.Next(min, max);
    public static double Double() => _rnd.NextDouble();
    public static double Double(double max) => _rnd.NextDouble() * max;
    public static double Double(double min, double max) => min + (_rnd.NextDouble() * (max - min));
    public static PointI PointI(SizeI s) => PointI(s.Width, s.Height);
    public static PointI PointI(int maxX, int maxY) => new PointI(_rnd.Next(maxX), _rnd.Next(maxY));
    public static Point Point(SizeI s) => Point(s.Width, s.Height);
    public static Point Point(double maxX, double maxY) => new Point(_rnd.NextDouble() * maxX, _rnd.NextDouble() * maxY);
}
