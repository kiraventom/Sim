using System;

namespace Sim.Geometry;

public readonly struct PointI(int x, int y)
{
    public static PointI INVALID { get; } = new PointI(int.MinValue, int.MinValue);
    public static PointI ZERO { get; } = new PointI(0, 0);

    public int X { get; } = x;
    public int Y { get; } = y;
    private readonly string _str = $"[{x},{y}]";

    public static PointI operator +(PointI a, PointI b) => new PointI(a.X + b.X, a.Y + b.Y);
    public static PointI operator -(PointI a, PointI b) => new PointI(a.X - b.X, a.Y - b.Y);
    public static Point operator *(PointI a, Point b) => new Point(a.X * b.X, a.Y * b.Y);
    public static Point operator *(PointI p, double d) => new Point(p.X * d, p.Y * d);

    public static bool operator ==(PointI a, PointI b) => a.X == b.X && a.Y == b.Y;
    public static bool operator !=(PointI a, PointI b) => a.X != b.X || a.Y != b.Y;

    public double Length => IsZero() ? 0 : Math.Sqrt(X * X + Y * Y);

    public bool IsZero() => X == 0 && Y == 0;

    public Point Normalize()
    {
        if (IsZero())
            return Point.ZERO;

        double len = Length;
        return new Point(X / len, Y / len);
    }

    public override string ToString() => _str;

    public override bool Equals(object obj) => obj is PointI point && this == point;
    public override int GetHashCode() => HashCode.Combine(X, Y);
}
