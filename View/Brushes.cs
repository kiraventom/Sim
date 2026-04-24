using System;
using Sim.Host;
using SkiaSharp;

namespace Sim.View;

public static class Brushes
{
   public static SKPaint Invalid => new SKPaint { Color = SKColors.Red };
   public static SKPaint Unknown => new SKPaint { Color = SKColors.Pink };
   public static SKPaint Info => new SKPaint { Color = SKColors.White };
   public static SKPaint Human => new SKPaint { Color = SKColors.LightGreen };
   public static SKPaint Obstacle => new SKPaint { Color = SKColors.Brown };
   public static SKPaint Line => new SKPaint 
   { 
       Color = SKColors.Blue.WithAlpha(80), 
       IsStroke = true,
       PathEffect = SKPathEffect.CreateDash([ 5, 5 ], 0)
   };

   public static SKPaint Background => new SKPaint { Color = new SKColor(0x22, 0x22, 0x22) };
   public static SKPaint Visor => new SKPaint 
   { 
        Color = new SKColor(0xEE, 0x33, 0x33), 
        IsStroke = true,
        PathEffect = SKPathEffect.CreateDash([ 5, 5 ], 0)
   };

    internal static SKPaint GetBrush(IEntity entity, bool isSelected)
    {
        var brush = entity switch
        {
            null => Brushes.Invalid,
            IHumanEntity => Brushes.Human,
            ILineEntity => Brushes.Line,
            IObstacleEntity => Brushes.Obstacle,
            _ => Brushes.Unknown
        };

        if (isSelected)
        {
            brush.Color.ToHsl(out var h, out var s, out var l);
            l = Math.Min(100f, l + 15);
            brush.Color = SKColor.FromHsl(h, s, l);
        }

        return brush;
    }
}
