using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical;

public class Pipe : Block
{
    public Pipe(Point arrayPosition, byte xx, byte yy):base(arrayPosition, xx, yy)
    {
        switch (xx)
        {
            case 0x02:
                if (Statics.Brandom.Next(0, 2) == 0) 
                    Texture = LevelTextures.PipeHorizontal;
                else
                    Texture = LevelTextures.PipeHorizontalAlt;
                break;
            case 0x03:
                if (Statics.Brandom.Next(0, 2) == 0)
                    Texture = LevelTextures.PipeVertical;
                else
                    Texture = LevelTextures.PipeVerticalAlt;
                break;
            case 0x04: Texture = LevelTextures.PipeCross; break;
            default: throw new Exception("Unhandeled");
        }
    }
}