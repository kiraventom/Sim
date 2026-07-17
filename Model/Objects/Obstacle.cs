using Sim.Geometry;
using Sim.Utils;

namespace Sim.Model.Objects;

internal class Obstacle : SimObject
{
    public Obstacle(int id) : base(id, GenSize())
    {
    }

    private static Size GenSize()
    {
        var size = RND.Double(0.1, 0.3);
        return new Size(size, size);
    }
}
