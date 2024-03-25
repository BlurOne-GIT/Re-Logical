using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical;

public class EmptyBlock : Block
{
    public EmptyBlock(Point arrayPosition, byte xx, byte yy):base(arrayPosition, xx, yy)
    {
        if (Statics.Brandom.Next(0, 2) == 0)
            _texture = LevelTextures.EmptyBlock;
        else
            _texture = LevelTextures.EmptyBlockAlt;
    }
}