using System;

namespace Sim.Geometry;

public readonly struct Size(double width, double height)
{
    public static Size INVALID { get; } = new Size(double.NaN, double.NaN);

    public double Width { get; } = width;
    public double Height { get; } = height;

    public static Size operator *(Size a, Size b) => new Size(a.Width * b.Width, a.Height * b.Height);
    public static Size operator *(Size a, double d) => new Size(a.Width * d, a.Height * d);
    public static bool operator ==(Size a, Size b) => a.Width == b.Width && a.Height == b.Height;
    public static bool operator !=(Size a, Size b) => a.Width != b.Width || a.Height != b.Height;

    public bool IsInvalid() => double.IsNaN(Width) || double.IsNaN(Height);

    public Point ToPoint() => new Point(Width, Height);
    public SizeI ToSizeI() => new SizeI((int)Math.Round(Width), (int)Math.Round(Height));
    public SizeI ToValidSizeI() => new SizeI(Math.Max((int)Math.Round(Width), 1), Math.Max((int)Math.Round(Height), 1));

    private readonly string _str = $"{width}x{height}";
    public override string ToString() => _str;

    public override bool Equals(object obj) => obj is Size s && this == s;

    public override int GetHashCode() => HashCode.Combine(Width, Height);
}


