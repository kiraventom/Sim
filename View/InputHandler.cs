using Avalonia.Input;
using Sim.Host;

namespace Sim.View;

public class InputHandler(MainWindow window, IWorldHost worldHost, Renderer renderer)
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
                renderer.ZoomIn();
                return;

            case Key.O:
                renderer.ZoomOut();
                return;
        }
    }
}
