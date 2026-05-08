using System;

namespace Sim.Geometry;

public readonly struct RaycastHit(int id, Point point)
{
    public static RaycastHit INVALID { get; } = new RaycastHit(-1, Point.INVALID);

    public int Id => id;
    public Point Enter => point;

    public bool IsInvalid() => Id < 0 || Enter.IsInvalid();

    public override bool Equals(object obj) => obj is RaycastHit h && h.Id == Id && h.Enter == Enter;
    public override int GetHashCode() => HashCode.Combine(Id, Enter);

    public override string ToString() => $"[ {Enter.ToString()} ]";
}
