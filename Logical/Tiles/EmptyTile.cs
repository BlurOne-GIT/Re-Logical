using Microsoft.Xna.Framework;

namespace Logical.Tiles;

public class EmptyTile : GameTile
{
    public EmptyTile(Game game, Point arrayPosition, byte xx, byte yy)
        : base(game, "EmptyTile", arrayPosition, xx, yy) =>
        DefaultSource = new Rectangle(Statics.Brandom.Next(3) * 36, 0, 36, 36);

    protected override void UnloadContent()
    {
        Game.Content.UnloadAsset(Texture.Name);
        base.UnloadContent();
    }
}