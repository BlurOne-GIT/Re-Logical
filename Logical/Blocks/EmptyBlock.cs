using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical.Blocks;

public class EmptyBlock : Block
{
    public EmptyBlock(Game game, Point arrayPosition, byte xx, byte yy)
        : base(game, "EmptyBlock", arrayPosition, xx, yy)
    {
        DefaultSource = new Rectangle(Statics.Brandom.Next(3) * 36, 0, 36, 36);
    }

    protected override void UnloadContent()
    {
        Game.Content.UnloadAsset(Texture.Name);
        base.UnloadContent();
    }
}