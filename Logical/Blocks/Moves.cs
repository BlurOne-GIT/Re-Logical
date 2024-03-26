using System;
using Logical.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical;

public class Moves : Block
{
    #region Field
    private readonly Vector2 bluePos1 = new Vector2(7f, 9f);
    private readonly Vector2 bluePos2 = new Vector2(7f, 14f);
    private readonly Vector2 bluePos3 = new Vector2(7f, 19f);
    private readonly Vector2 bluePos4 = new Vector2(7f, 24f);
    #endregion

    public Moves(Point arrayPosition, byte xx, byte yy):base(arrayPosition, xx, yy)
    {
        Texture = LevelTextures.Moves;
    }

    public override void Render(SpriteBatch _spriteBatch)
    {
        base.Render(_spriteBatch);
        if (LevelState.MovesLeft < 4)
        {
            _spriteBatch.Draw(
                LevelTextures.MovesBlue,
                (_position + bluePos1) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.1f
            );
        }
        if (LevelState.MovesLeft < 3)
        {
            _spriteBatch.Draw(
                LevelTextures.MovesBlue,
                (_position + bluePos2) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.1f
            );
        }
        if (LevelState.MovesLeft < 2)
        {
            _spriteBatch.Draw(
                LevelTextures.MovesBlue,
                (_position + bluePos3) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.1f
            );
        }
        if (LevelState.MovesLeft == 0)
        {
            _spriteBatch.Draw(
                LevelTextures.MovesBlue,
                (_position + bluePos4) * Configs.Scale,
                null,
                Color.White * Statics.Opacity,
                0,
                Vector2.Zero,
                Configs.Scale,
                SpriteEffects.None,
                0.1f
            );
        }
    }
}