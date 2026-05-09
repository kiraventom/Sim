using System;
using System.Collections.Generic;

namespace Sim.Geometry;

public readonly struct Rect
{
    public static Rect INVALID { get; } = new Rect(Point.INVALID, Size.INVALID);

    public Point Pos { get; }
    public Size Size { get; }

    public double Left => Pos.X - Size.Width / 2;
    public double Right => Pos.X + Size.Width / 2;
    public double Top => Pos.Y - Size.Height / 2;
    public double Bottom => Pos.Y + Size.Height / 2;

    public Point Center => Pos;
    public Point TopLeft => new Point(Left, Top);
    public Point TopRight => new Point(Right, Top);
    public Point BottomLeft => new Point(Left, Bottom);
    public Point BottomRight => new Point(Right, Bottom);

    public double Width => Size.Width;
    public double Height => Size.Height;

    public Rect(Point pos, Size size)
    {
        Pos = pos;
        Size = size;
    }

    public Rect(double left, double top, double right, double bottom)
    {
        Size = new Size(right - left, bottom - top);
        Pos = new Point(left + Size.Width / 2, top + Size.Height / 2);
    }

    public static bool operator ==(Rect a, Rect b) => a.Pos == b.Pos && a.Size == b.Size;
    public static bool operator !=(Rect a, Rect b) => a.Pos != b.Pos || a.Size != b.Size;

    public IEnumerable<Point> GetPoints()
    {
        yield return TopLeft;
        yield return TopRight;
        yield return BottomLeft;
        yield return BottomRight;
    }

    public bool IsInvalid() => Pos.IsInvalid() || Size.IsInvalid();

    public Rect Offset(Point offset) => new Rect(Pos + offset, Size);

    public bool Intersects(Rect rect) => Left < rect.Right && Right > rect.Left && Top < rect.Bottom && Bottom > rect.Top;

    public bool Contains(Point p) => p.X >= Left && p.X <= Right && p.Y >= Top && p.Y <= Bottom;

    public static bool ClampToMap(ref Rect rect)
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

        rect = new Rect(new Point(x + width / 2, y + width / 2), new Size(width, height));
        return oldRect != rect;
    }

    public RectI ToRectI() => new RectI(TopLeft.ToPointI(), Size.ToSizeI());

    public override string ToString() => $"[{Pos.X}:{Pos.Y} {Size.Width}x{Size.Height}]";

    public override bool Equals(object obj) => obj is Rect r && r == this;
    public override int GetHashCode() => HashCode.Combine(Pos, Size);

    internal static (Point, Point) GetDirectVector(Rect a, Rect b)
    {
        var (x1, x2) = GetClosest1D(a.Left, a.Right, b.Left, b.Right);
        var (y1, y2) = GetClosest1D(a.Top, a.Bottom, b.Top, b.Bottom);

        return (new Point(x1, y1), new Point(x2, y2));
    }

    private static (double, double) GetClosest1D(double aMin, double aMax, double bMin, double bMax)
    {
        if (aMax <= bMin) 
            return (aMax, bMin);

        if (bMax <= aMin) 
            return (aMin, bMax);

        double overlapCenter = (Math.Max(aMin, bMin) + Math.Min(aMax, bMax)) / 2.0;
        return (overlapCenter, overlapCenter);
    }}
