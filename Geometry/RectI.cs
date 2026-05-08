using System;

namespace Sim.Geometry;

public readonly struct RectI(PointI pos, SizeI size)
{
    public PointI TopLeft { get; } = pos;
    public SizeI Size { get; } = size;

    public int Left => TopLeft.X;
    public int Right => TopLeft.X + Size.Width;
    public int Top => TopLeft.Y;
    public int Bottom => TopLeft.Y + Size.Height;
    public PointI Center => new PointI(Left + Width / 2, Top + Height / 2);

    public int Width => Size.Width;
    public int Height => Size.Height;

    public static bool operator ==(RectI a, RectI b) => a.TopLeft == b.TopLeft && a.Size == b.Size;
    public static bool operator !=(RectI a, RectI b) => a.TopLeft != b.TopLeft || a.Size != b.Size;

    private readonly string _str = $"[{pos.X}:{pos.Y} {size.Width}x{size.Height}]";
    public override string ToString() => _str;

    public override bool Equals(object obj) => obj is RectI r && r == this;
    public override int GetHashCode() => HashCode.Combine(TopLeft, Size);
}
