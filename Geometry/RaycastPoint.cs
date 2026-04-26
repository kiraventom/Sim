using System;

namespace Sim.Geometry;

public readonly struct RaycastHit(int id, Point enter, Point exit)
{
    public int Id => id;
    public Point Enter => enter;
    public Point Exit => exit;

    public override bool Equals(object obj) => obj is RaycastHit h && h.Id == Id && h.Enter == Enter && h.Exit == Exit;
    public override int GetHashCode() => HashCode.Combine(Id, Enter, Exit);

    public override string ToString() => $"[ {Enter.ToString()} -> {Exit.ToString()} ]";
}
