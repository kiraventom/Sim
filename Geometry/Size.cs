using System;

namespace Sim.Geometry;

public readonly struct SizeI(int width, int height)
{
    public int Width { get; } = width;
    public int Height { get; } = height;

    public PointI ToPointI() => new PointI(Width, Height);
}

public readonly struct Size(double width, double height)
{
    public double Width { get; } = width;
    public double Height { get; } = height;

    public SizeI ToSizeI() => new SizeI((int)Math.Round(Width), (int)Math.Round(Height));
    public SizeI ToValidSizeI() => new SizeI(Math.Max((int)Math.Round(Width), 1), Math.Max((int)Math.Round(Height), 1));
}


