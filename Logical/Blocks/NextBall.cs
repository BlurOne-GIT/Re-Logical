using System;
using Logical.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical;

public class NextBall : Block
{
    #region Field
    private readonly Vector2 holderPos = new Vector2(9f);
    private readonly Vector2 indicatorPos = new Vector2(12f);
    private readonly Vector2 shadowPos = new Vector2(12f, 13f);
    #endregion

    public NextBall(Point arrayPosition, byte xx, byte yy):base(arrayPosition, xx, yy)
    {
        Texture = LevelTextures.EmptyBlock;
    }

    public override void Render(SpriteBatch _spriteBatch)
    {
        base.Render(_spriteBatch);
        _spriteBatch.Draw(
            LevelTextures.HolderShadowEmpty,
            (_position + shadowPos) * Configs.Scale,
            null,
            Color.White * Statics.Opacity,
            0,
            Vector2.Zero,
            Configs.Scale,
            SpriteEffects.None,
            0.1f
        );
        _spriteBatch.Draw(
            LevelTextures.Holder,
            (_position + holderPos) * Configs.Scale,
            null,
            Color.White * Statics.Opacity,
            0,
            Vector2.Zero,
            Configs.Scale,
            SpriteEffects.None,
            0.2f
        );
        _spriteBatch.Draw(
            LevelTextures.Indicator[(int)LevelState.NextBall],
            (_position + indicatorPos) * Configs.Scale,
            null,
            Color.White * Statics.Opacity,
            0,
            Vector2.Zero,
            Configs.Scale,
            SpriteEffects.None,
            0.3f
        );
    }
}