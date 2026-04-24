using System;

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

    public static bool operator ==(Rect a, Rect b) => a.Pos == b.Pos && a.Size == b.Size;
    public static bool operator !=(Rect a, Rect b) => a.Pos != b.Pos || a.Size != b.Size;

    public bool IsInvalid() => Pos.IsInvalid() || Size.IsInvalid();

    public Rect Offset(Point offset) => new Rect(Pos + offset, Size);

    public static bool EnsureValid(ref Rect rect)
    {
        var oldRect = rect;

        var x = Math.Max(0, rect.Left);
        var y = Math.Max(0, rect.Top);

        var width = Math.Max(0.0001, rect.Width);
        var height = Math.Max(0.0001, rect.Height);

        if (width != height)
            width = height;

        if (x + width > 1.0)
            x = 1.0 - width;

        if (y + height > 1.0)
            y = 1.0 - height;

        rect = new Rect(new Point(x, y), new Size(width, height));
        return oldRect == rect;
    }

    public RectI ToRectI() => new RectI(Pos.ToPointI(), Size.ToSizeI());

    public bool Intersects(Rect rect) => Left < rect.Right && Right > rect.Left && Top < rect.Bottom && Bottom > rect.Top;

    private readonly string _str = $"[{pos.X}:{pos.Y} {size.Width}x{size.Height}]";
    public override string ToString() => _str;

    public override bool Equals(object obj) => obj is Rect r && r == this;

    public override int GetHashCode() => HashCode.Combine(Pos, Size);
}
