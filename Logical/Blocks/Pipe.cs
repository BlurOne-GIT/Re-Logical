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
                    _texture = LevelTextures.PipeHorizontal;
                else
                    _texture = LevelTextures.PipeHorizontalAlt;
                break;
            case 0x03:
                if (Statics.Brandom.Next(0, 2) == 0)
                    _texture = LevelTextures.PipeVertical;
                else
                    _texture = LevelTextures.PipeVerticalAlt;
                break;
            case 0x04: _texture = LevelTextures.PipeCross; break;
            default: throw new Exception("Unhandeled");
        }
    }
}