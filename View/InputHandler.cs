using Avalonia.Input;
using Sim.Geometry;
using Sim.Host;

namespace Sim.View;

public class InputHandler(MainWindow window, IWorldHost worldHost, MapRenderer renderer)
{
    public void Handle(KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.C when e.KeyModifiers.HasFlag(KeyModifiers.Control):
                window.Close();
                return;

            case Key.Space:
                worldHost.TogglePause();
                return;

            case Key.I:
                renderer.ZoomCalc.ZoomIn(renderer.PanCalc);
                return;

            case Key.O:
                renderer.ZoomCalc.ZoomOut(renderer.PanCalc);
                return;

            case Key.H:
                renderer.PanCalc.Move(new Point(-0.2, 0));
                return;

            case Key.L:
                renderer.PanCalc.Move(new Point(0.2, 0));
                return;

            case Key.K:
                renderer.PanCalc.Move(new Point(0, -0.2));
                return;

            case Key.J:
                renderer.PanCalc.Move(new Point(0, 0.2));
                return;

            case Key.R:
                renderer.PanCalc.Reset();
                renderer.ZoomCalc.Reset();
                return;

            case Key.N when e.KeyModifiers.HasFlag(KeyModifiers.Shift):
                worldHost.SelectPrevObject();
                return;

            case Key.N:
                worldHost.SelectNextObject();
                return;

            case Key.Escape:
                worldHost.UnselectObject();
                return;

            case Key.P:
                renderer.ToggleLines();
                return;
        }
    }
}
