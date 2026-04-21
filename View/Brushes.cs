using Sim.Host;
using SkiaSharp;

namespace Sim.View;

public static class Brushes
{
   public static SKPaint Invalid => new SKPaint { Color = SKColors.Red };
   public static SKPaint Unknown => new SKPaint { Color = SKColors.Pink };
   public static SKPaint Human => new SKPaint { Color = SKColors.LightGreen };
   public static SKPaint Background => new SKPaint { Color = new SKColor(0x22, 0x22, 0x22) };
   public static SKPaint Visor => new SKPaint 
   { 
        Color = new SKColor(0xEE, 0x33, 0x33), 
        IsStroke = true,
        PathEffect = SKPathEffect.CreateDash([ 5, 5 ], 0)
   };

    internal static SKPaint GetBrush(ObjectType type)
    {
        return type switch
        {
            ObjectType.Invalid => Brushes.Invalid,
            ObjectType.Human => Brushes.Human,
            _ => Brushes.Unknown
        };
    }
}
