using Sim.Geometry;
using Sim.Model.Entities;

namespace Sim.Host;

public interface IWorldHost
{
    SizeI WorldSize { get; }

    int SelectedObjectId { get; }

    void SelectNextObject();
    void SelectPrevObject();

    string GetInfo(int id);
    string GetWorldInfo();

    void TogglePause();

    void SpeedUp();
    void SlowDown();
    void ResetSpeed();
    
    void UpdateSnapshot(EntitySnapshot snapshot);
    void UnselectObject();
}
