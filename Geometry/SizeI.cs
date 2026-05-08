using System;

namespace Sim.Geometry;

public readonly struct SizeI(int width, int height)
{
    public int Width { get; } = width;
    public int Height { get; } = height;

    public static bool operator ==(SizeI a, SizeI b) => a.Width == b.Width && a.Height == b.Height;
    public static bool operator !=(SizeI a, SizeI b) => a.Width != b.Width || a.Height != b.Height;

    public PointI ToPointI() => new PointI(Width, Height);

    public override bool Equals(object obj) => obj is SizeI s && this == s;
    public override int GetHashCode() => HashCode.Combine(Width, Height);
}
