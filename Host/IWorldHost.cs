using Sim.Geometry;
using Sim.Model.Entities;

namespace Sim.Host;

public interface IWorldHost
{
    SizeI WorldSize { get; }

    int SelectedObjectId { get; }

    void SelectNextObject();
    void SelectPrevObject();
    bool SelectObject(int id);

    string GetInfo(int id);

    void TogglePause();
    
    EntitySnapshot GetSnapshot(); 
}
