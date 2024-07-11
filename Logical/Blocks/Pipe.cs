using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical.Blocks;

public class Pipe : Block, IFixable
{
    public Pipe(Game game, Point arrayPosition, byte xx, byte yy) : base
    (
        game,
        game.Content.Load<Texture2D>($"{Configs.GraphicSet}/{TextureSwitcher(xx)}"),
        arrayPosition, xx, yy
    )
    {
        DefaultRectangle = new Rectangle(Statics.Brandom.Next(0, 2) * 36, 0, 36, 36);
    }

    private static string TextureSwitcher(byte xx) => (xx % 3) switch
    {
        2 => "PipeHorizontal",
        0 => "PipeVertical",
        1 => "PipeCross",
        _ => throw new ArgumentException("Invalid Pipe direction")
    };

    public void Fix()
    {
        var rectangle = DefaultRectangle!.Value;
        rectangle.X += 36;
        DefaultRectangle = rectangle;
    }

    public bool ShallFix()
    {
        var xx = FileValue % 3;
        return xx is 2 && Configs.GraphicSet is 1 || xx is 0 && Configs.GraphicSet is 1 or >= 3;
    }
}