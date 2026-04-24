namespace Sim.Geometry;

public readonly struct Rect(Point pos, Size size)
{
    public static Rect INVALID { get; } = new Rect(Point.INVALID, Size.INVALID);

    public Point Pos { get; } = pos;
    public Size Size { get; } = size;

    public double Left => Pos.X;
    public double Right => Pos.X + Size.Width;
    public double Top => Pos.Y;
    public double Bottom => Pos.Y + Size.Height;

    public double Width => Size.Width;
    public double Height => Size.Height;

    public bool IsInvalid() => Pos.IsInvalid() || Size.IsInvalid();

    public Rect Offset(Point offset) => new Rect(Pos + offset, Size);

    public RectI ToRectI() => new RectI(Pos.ToPointI(), Size.ToSizeI());

    public bool Intersects(Rect rect) => Left < rect.Right && Right > rect.Left && Top < rect.Bottom && Bottom > rect.Top;

    private readonly string _str = $"[{pos.X}:{pos.Y} {size.Width}x{size.Height}]";
    public override string ToString() => _str;
}
