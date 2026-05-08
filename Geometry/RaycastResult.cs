namespace Sim.Geometry;

public struct RaycastResult(RaycastHit hit)
{
    public static RaycastResult NO_HITS { get; } = new RaycastResult(RaycastHit.INVALID);

    public RaycastHit Hit => hit;

    public bool HasHit() => !Hit.IsInvalid();
}

