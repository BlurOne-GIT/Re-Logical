using Logical.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical.Blocks;

public class Moves : Block
{
    #region Field
    private static Texture2D _blue;
    private static readonly Vector2[] BlueOffsets =
    {
        new(7f, 9f),
        new(7f, 14f),
        new(7f, 19f),
        new(7f, 24f)
    };
    #endregion

    public Moves(Game game, Point arrayPosition, byte xx, byte yy)
        : base(game, "Moves", arrayPosition, xx, yy) { }

    protected override void LoadContent()
    {
        _blue ??= Game.Content.Load<Texture2D>("MovesBlue");
        base.LoadContent();
    }

    protected override void UnloadContent()
    {
        _blue = null;
        Game.Content.UnloadAsset("MovesBlue");
        base.UnloadContent();
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);

        for (int i = 0; i < 4 - LevelState.MovesLeft; i++)
            DrawAnotherTexture(_blue, BlueOffsets[i], 1);
    }
}