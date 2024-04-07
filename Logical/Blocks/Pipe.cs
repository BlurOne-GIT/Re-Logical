using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical.Blocks;

public class Pipe : Block
{
    public Pipe(Game game, Point arrayPosition, byte xx, byte yy, bool randomize = true) 
        : base(game, TextureSwitcher(xx, randomize), arrayPosition, xx, yy) { }

    private static Texture2D TextureSwitcher(byte xx, bool randomize) => (xx % 3) switch
    {
        2 => !randomize || Statics.Brandom.Next(0, 2) == 0 ? LevelResources.PipeHorizontal : LevelResources.PipeHorizontalAlt,
        0 => !randomize || Statics.Brandom.Next(0, 2) == 0 ? LevelResources.PipeVertical : LevelResources.PipeVerticalAlt,
        1 => LevelResources.PipeCross,
        _ => throw new ArgumentException("Invalid Pipe direction")
    };
}