using Microsoft.Xna.Framework;

namespace Logical.Blocks;

public class EmptyBlock : Block
{
    public EmptyBlock(Game game, Point arrayPosition, byte xx, byte yy)
        : base(game, Statics.Brandom.Next(0, 2) == 0 ? LevelResources.EmptyBlock : LevelResources.EmptyBlockAlt, arrayPosition, xx, yy) { }
}