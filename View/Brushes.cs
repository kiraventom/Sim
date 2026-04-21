using SkiaSharp;

namespace Sim.View;

public static class Brushes
{
   public static SKPaint Human => new SKPaint { Color = SKColors.LightGreen };
   public static SKPaint Background => new SKPaint { Color = new SKColor(0x22, 0x22, 0x22) };
   public static SKPaint Visor => new SKPaint { Color = new SKColor(0xEE, 0x33, 0x33), IsStroke = true };
}


