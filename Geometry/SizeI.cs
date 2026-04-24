namespace Sim.Geometry;

public readonly struct SizeI(int width, int height)
{
    public int Width { get; } = width;
    public int Height { get; } = height;

    public PointI ToPointI() => new PointI(Width, Height);
}



