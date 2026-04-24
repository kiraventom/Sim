using Sim.Geometry;
using Sim.Utils;

namespace Sim.Model.Objects;

internal class Obstacle : SimObject
{
    public override Size Size { get; }

    public Obstacle(int id) : base(id)
    {
        var size = RND.Double(0.1, 0.3);
        Size = new Size(size, size);
    }
}

