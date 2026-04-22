using System.Collections.Generic;
using Sim.Geometry;

namespace Sim.Host;

public interface IWorldHost
{
    SizeI WorldSize { get; }

    int SelectedObjectId { get; }

    void SelectNextObject();
    void SelectPrevObject();
    bool SelectObject(int id);

    IObjectInfo GetInfo(int id);

    void TogglePause();
    IReadOnlyList<IEntity> GetEntities();
}
