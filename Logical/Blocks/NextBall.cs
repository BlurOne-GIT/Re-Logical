using Logical.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical.Blocks;

public class NextBall : Block
{
    #region Field
    private readonly Vector2 _holderPos = new(9f);
    private readonly Vector2 _indicatorPos = new(12f);
    private readonly Vector2 _shadowPos = new(12f, 13f);
    #endregion

    public NextBall(Game game, Point arrayPosition, byte xx, byte yy):base(game, LevelResources.EmptyBlock, arrayPosition, xx, yy) { }

    public override void Draw(GameTime gameTime)
    
    {
        base.Draw(gameTime);
        var spriteBatch = Game.Services.GetService<SpriteBatch>();
        spriteBatch.Draw(
            LevelResources.HolderShadowEmpty,
            (Position + _shadowPos) * Configs.Scale,
            null,
            Color.White * Statics.Opacity,
            0,
            Vector2.Zero,
            Configs.Scale,
            SpriteEffects.None,
            0.1f
        );
        spriteBatch.Draw(
            LevelResources.Holder,
            (Position + _holderPos) * Configs.Scale,
            null,
            Color.White * Statics.Opacity,
            0,
            Vector2.Zero,
            Configs.Scale,
            SpriteEffects.None,
            0.2f
        );
        spriteBatch.Draw(
            LevelResources.Indicator[(int)LevelState.NextBall],
            (Position + _indicatorPos) * Configs.Scale,
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