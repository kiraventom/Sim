using SkiaSharp;

namespace Sim.View;

public static class Brushes
{
   public static SKPaint Human { get; } = new SKPaint { Color = SKColors.LightGreen };
   public static SKPaint Empty { get; } = new SKPaint { Color = new SKColor(0x22, 0x22, 0x22) };
}


