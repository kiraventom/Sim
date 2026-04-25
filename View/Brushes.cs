using System;
using System.Collections.Generic;
using SkiaSharp;

namespace Sim.View;

public static class Fonts
{
    public static SKFont Info { get; } = new SKFont(SKTypeface.CreateDefault());
}

public static class Brushes
{
    private static readonly Dictionary<SKColor, SKPaint> _cache = [];

    public static SKPaint Invalid { get; } = new SKPaint { Color = SKColors.Red };
    public static SKPaint Unknown { get; } = new SKPaint { Color = SKColors.Pink };
    public static SKPaint Info { get; } = new SKPaint { Color = SKColors.White };
    public static SKPaint Line { get; } = new SKPaint { Color = SKColors.SkyBlue.WithAlpha(70), IsStroke = true, PathEffect = SKPathEffect.CreateDash([ 5, 5 ], 0) };
    public static SKPaint Human { get; } = new SKPaint { Color = SKColors.LightGreen };
    public static SKPaint Obstacle { get; } = new SKPaint { Color = SKColors.Brown };
    public static SKPaint Area { get; } = new SKPaint { Color = SKColors.White.WithAlpha(70) };
    public static SKPaint SelectedLine { get; } = ApplySelection(Line);
    public static SKPaint SelectedHuman { get; } = ApplySelection(Human);
    public static SKPaint SelectedObstacle { get; } = ApplySelection(Obstacle);
    public static SKPaint Background { get; } = new SKPaint { Color = new SKColor(0x22, 0x22, 0x22) };
    public static SKPaint Visor { get; } = new SKPaint { Color = new SKColor(0xEE, 0x33, 0x33), IsStroke = true, PathEffect = SKPathEffect.CreateDash([ 5, 5 ], 0) };

    public static SKPaint ChangeColor(SKPaint paint, SKColor color)
    {
        if (!_cache.TryGetValue(color, out var cached))
        {
            cached = paint.Clone();
            cached.Color = color;
            _cache[color] = cached;
        }

        return cached;
    }

    private static SKPaint ApplySelection(SKPaint brush)
    {
        var clone = brush.Clone();
        clone.Color.ToHsl(out var h, out var s, out var l);
        l = Math.Min(100f, l + 15);
        clone.Color = SKColor.FromHsl(h, s, l);
        return clone;
    }
}
