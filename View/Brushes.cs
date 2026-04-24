using System;
using SkiaSharp;

namespace Sim.View;

public static class Brushes
{
    public static SKPaint Invalid => new SKPaint { Color = SKColors.Red };
    public static SKPaint Unknown => new SKPaint { Color = SKColors.Pink };
    public static SKPaint Info => new SKPaint { Color = SKColors.White };

    public static SKPaint GetHumanBrush(bool isSelected) => ApplySelection(new SKPaint { Color = SKColors.LightGreen }, isSelected);
    public static SKPaint GetObstacleBrush(bool isSelected) => ApplySelection(new SKPaint { Color = SKColors.Brown }, isSelected);
    public static SKPaint GetAreaBrush() => new SKPaint { Color = SKColors.White.WithAlpha(70) };
    
    public static SKPaint GetLineBrush(bool isSelected)
    {
        var paint = new SKPaint 
        { 
            Color = SKColors.Blue.WithAlpha(80), 
            IsStroke = true,
            PathEffect = SKPathEffect.CreateDash([ 5, 5 ], 0)
        };
        return ApplySelection(paint, isSelected);
    }

    public static SKPaint Background => new SKPaint { Color = new SKColor(0x22, 0x22, 0x22) };
    public static SKPaint Visor => new SKPaint 
    { 
        Color = new SKColor(0xEE, 0x33, 0x33), 
        IsStroke = true,
        PathEffect = SKPathEffect.CreateDash([ 5, 5 ], 0)
    };

    private static SKPaint ApplySelection(SKPaint brush, bool isSelected)
    {
        if (isSelected)
        {
            brush.Color.ToHsl(out var h, out var s, out var l);
            l = Math.Min(100f, l + 15);
            brush.Color = SKColor.FromHsl(h, s, l);
        }

        return brush;
    }
}
