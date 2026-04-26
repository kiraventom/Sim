using System;

namespace Sim.Geometry;

public readonly struct Point(double x, double y)
{
    public static Point INVALID { get; } = new Point(double.NaN, double.NaN);
    public static Point ZERO { get; } = new Point(0, 0);

    public double X { get; } = x;
    public double Y { get; } = y;

    public static Point operator +(Point a, Point b) => new Point(a.X + b.X, a.Y + b.Y);
    public static Point operator -(Point a, Point b) => new Point(a.X - b.X, a.Y - b.Y);
    public static Point operator *(Point a, Point b) => new Point(a.X * b.X, a.Y * b.Y);
    public static Point operator *(Point p, double d) => new Point(p.X * d, p.Y * d);
    public static bool operator ==(Point a, Point b) => a.X == b.X && a.Y == b.Y;
    public static bool operator !=(Point a, Point b) => a.X != b.X || a.Y != b.Y;

    public static Point operator +(Point a, Size b) => new Point(a.X + b.Width, a.Y + b.Height);

    public double Length => IsZero() ? 0 : Math.Sqrt(X * X + Y * Y);

    public bool IsZero() => X == 0 && Y == 0;
    public bool IsInvalid() => double.IsNaN(X) || double.IsNaN(Y);
    public bool IsOnMap() => X >= 0 && X <= 1 && Y >= 0 && Y <= 1;

    public Point Normalize()
    {
        if (IsZero())
            return Point.ZERO;

        double len = Length;
        return new Point(X / len, Y / len);
    }

    public Size ToSize() => new Size(X, Y);
    public PointI ToPointI() => new PointI((int)Math.Round(X), (int)Math.Round(Y));

    public override string ToString() => $"[{X},{Y}]";

    public override bool Equals(object obj) => obj is Point point && this == point;
    public override int GetHashCode() => HashCode.Combine(X, Y);
}

