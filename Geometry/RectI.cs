namespace Sim.Geometry;

public readonly struct RectI(PointI pos, SizeI size)
{
    public PointI Pos { get; } = pos;
    public SizeI Size { get; } = size;

    public int Left => Pos.X;
    public int Right => Pos.X + Size.Width;
    public int Top => Pos.Y;
    public int Bottom => Pos.Y + Size.Height;

    public int Width => Size.Width;
    public int Height => Size.Height;

    private readonly string _str = $"[{pos.X}:{pos.Y} {size.Width}x{size.Height}]";
    public override string ToString() => _str;
}
