using System;

namespace Sim.Geometry;

public readonly struct Point(double x, double y)
{
    public static Point ZERO { get; } = new Point(0, 0);

    public double X { get; } = x;
    public double Y { get; } = y;

    public static Point operator +(Point a, Point b) => new Point(a.X + b.X, a.Y + b.Y);
    public static Point operator -(Point a, Point b) => new Point(a.X - b.X, a.Y - b.Y);
    public static Point operator *(Point p, double d) => new Point(p.X * d, p.Y * d);

    public bool IsZero() => X == 0 && Y == 0;

    public PointI ToPointI() => new PointI((int)Math.Round(X), (int)Math.Round(Y));
}

