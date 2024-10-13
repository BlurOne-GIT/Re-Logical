using Logical.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical.Blocks;

public class MarbleDisplay(Game game, Point arrayPosition, byte xx, byte yy)
    : Block(game, "MarbleDisplay", arrayPosition, xx, yy)
{
    #region Field
    private static Texture2D _blue;
    private static readonly Vector2[] BlueOffsets =
    [
        new Vector2(7f, 9f),
        new Vector2(7f, 14f),
        new Vector2(7f, 19f),
        new Vector2(7f, 24f)
    ];
    #endregion

    protected override void LoadContent()
    {
        _blue ??= Game.Content.Load<Texture2D>("MarbleDisplayBlue");
        base.LoadContent();
    }

    protected override void UnloadContent()
    {
        _blue = null;
        Game.Content.UnloadAsset("MarbleDisplayBlue");
        base.UnloadContent();
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);

        for (int i = 0; i < 4 - LevelState.MovesLeft; i++)
            DrawAnotherTexture(_blue, BlueOffsets[i], 1);
    }
}