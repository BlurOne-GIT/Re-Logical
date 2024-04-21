using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical.Blocks;

public class Pipe : Block
{
    public Pipe(Game game, Point arrayPosition, byte xx, byte yy, bool randomize = true) 
        : base(game, game.Content.Load<Texture2D>($"{Configs.GraphicSet}/{TextureSwitcher(xx, randomize)}"), arrayPosition, xx, yy) { }

    private static string TextureSwitcher(byte xx, bool randomize) => (xx % 3) switch
    {
        2 => !randomize || Configs.GraphicSet is 1 || Statics.Brandom.Next(0, 2) == 0 ? "PipeHorizontal" : "PipeHorizontalAlt",
        0 => !randomize || Configs.GraphicSet is 1 or 3 || Statics.Brandom.Next(0, 2) == 0 ? "PipeVertical" : "PipeVerticalAlt",
        1 => "PipeCross",
        _ => throw new ArgumentException("Invalid Pipe direction")
    };
}