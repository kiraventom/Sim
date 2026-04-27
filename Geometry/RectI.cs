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


    private readonly string _str = $"[{pos.X}:{pos.Y} {size.Width}x{size.Height}]";
    public override string ToString() => _str;
}
