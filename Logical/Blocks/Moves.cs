using Logical.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical.Blocks;

public class Moves : Block
{
    #region Field
    private readonly Vector2 _bluePos1 = new(7f, 9f);
    private readonly Vector2 _bluePos2 = new(7f, 14f);
    private readonly Vector2 _bluePos3 = new(7f, 19f);
    private readonly Vector2 _bluePos4 = new(7f, 24f);
    #endregion

    public Moves(Game game, Point arrayPosition, byte xx, byte yy):base(game, LevelResources.Moves, arrayPosition, xx, yy) { }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        var spriteBatch = Game.Services.GetService<SpriteBatch>();
        switch (LevelState.MovesLeft)
        {
            case 0:
                spriteBatch.Draw(
                    LevelResources.MovesBlue,
                    (Position + _bluePos4) * Configs.Scale,
                    null,
                    Color.White * Statics.Opacity,
                    0,
                    Vector2.Zero,
                    Configs.Scale,
                    SpriteEffects.None,
                    0.1f
                );
                goto case 1;
            case 1:
                spriteBatch.Draw(
                    LevelResources.MovesBlue,
                    (Position + _bluePos3) * Configs.Scale,
                    null,
                    Color.White * Statics.Opacity,
                    0,
                    Vector2.Zero,
                    Configs.Scale,
                    SpriteEffects.None,
                    0.1f
                );
                goto case 2;
            case 2:
                spriteBatch.Draw(
                    LevelResources.MovesBlue,
                    (Position + _bluePos2) * Configs.Scale,
                    null,
                    Color.White * Statics.Opacity,
                    0,
                    Vector2.Zero,
                    Configs.Scale,
                    SpriteEffects.None,
                    0.1f
                );
                goto case 3;
            case 3:
                spriteBatch.Draw(
                    LevelResources.MovesBlue,
                    (Position + _bluePos1) * Configs.Scale,
                    null,
                    Color.White * Statics.Opacity,
                    0,
                    Vector2.Zero,
                    Configs.Scale,
                    SpriteEffects.None,
                    0.1f
                );
                break;
        }
    }
}