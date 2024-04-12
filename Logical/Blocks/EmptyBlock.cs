using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical.Blocks;

public class EmptyBlock : Block
{
    public EmptyBlock(Game game, Point arrayPosition, byte xx, byte yy)
        : base(game, game.Content.Load<Texture2D>($"{Configs.GraphicSet}/" +
                                                  (Statics.Brandom.Next(0, 2) == 0 ? "EmptyBlock" : "EmptyBlockAlt")), arrayPosition, xx, yy) { }

    protected override void UnloadContent()
    {
        Game.Content.UnloadAsset(Texture.Name);
        base.UnloadContent();
    }
}