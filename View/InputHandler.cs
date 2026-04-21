using Avalonia.Input;
using Sim.Host;
using SkiaSharp;

namespace Sim.View;

public class InputHandler(MainWindow window, IWorldHost worldHost, ZoomCalculator zoomCalc, PanCalculator panCalc)
{
    public void Handle(KeyEventArgs e)
    {
        const int MOVE_FACTOR = 5;

        var xMoveFactor = (float)(zoomCalc.Size.Width / MOVE_FACTOR);
        var yMoveFactor = (float)(zoomCalc.Size.Height / MOVE_FACTOR);

        switch (e.Key)
        {
            case Key.C when e.KeyModifiers.HasFlag(KeyModifiers.Control):
                window.Close();
                return;

            case Key.Space:
                worldHost.TogglePause();
                return;

            case Key.I:
                zoomCalc.ZoomIn();
                return;

            case Key.O:
                zoomCalc.ZoomOut();
                return;

            case Key.H:
                panCalc.Move(new SKPoint(-xMoveFactor, 0));
                return;

            case Key.L:
                panCalc.Move(new SKPoint(xMoveFactor, 0));
                return;

            case Key.K:
                panCalc.Move(new SKPoint(0, -yMoveFactor));
                return;

            case Key.J:
                panCalc.Move(new SKPoint(0, yMoveFactor));
                return;

            case Key.R:
                panCalc.Reset();
                zoomCalc.Reset();
                return;
        }
    }
}
