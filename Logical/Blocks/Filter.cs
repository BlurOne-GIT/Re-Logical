using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical;

public class Filter : Block, IUpdateable, IOverlayable
{
    #region Fields
    private readonly BallColors _color;
    private readonly Texture2D _shadow;
    private readonly Texture2D _ball;
    private readonly Vector2 ballPos = new Vector2(14f);
    private readonly Vector2 shadowPos = new Vector2(10f, 11f);
    #endregion

    public Filter(Point arrayPosition, byte xx, byte yy):base(arrayPosition, xx, yy)
    {
        switch (xx)
        {
            case 0x05:
                _texture = LevelTextures.PipeHorizontal;
                _shadow = LevelTextures.FilterShadowHorizontal;
                break;
            case 0x06:
                _texture = LevelTextures.PipeVertical;
                _shadow = LevelTextures.FilterShadowVertical;
                break;
            case 0x07:
                _texture = LevelTextures.PipeCross;
                _shadow = LevelTextures.FilterShadowCross;
                break;
            default: throw new Exception("Unhandeled");
        }
        _color = (BallColors)Argument-1;
        _ball = LevelTextures.SpinnerBall[Argument-1];
    }

    public void Update(GameTime gameTime)
    {
        foreach (Ball ball in Ball.allBalls)
        {
            if (ball.Position != Statics.DetectionPoint + _position)
                continue;

            if (ball.BallColor != _color)
                ball.Bounce();
        }
    }

    public override void Render(SpriteBatch _spriteBatch)
    {
        base.Render(_spriteBatch);
        _spriteBatch.Draw(
            _ball,
            (_position + ballPos) * Configs.Scale,
            null,
            Color.White * Statics.Opacity,
            0f,
            Vector2.Zero,
            Configs.Scale,
            SpriteEffects.None,
            0.1f
        );
        _spriteBatch.Draw(
            _shadow,
            (_position + shadowPos) * Configs.Scale,
            null,
            Color.White * Statics.Opacity,
            0,
            Vector2.Zero,
            Configs.Scale,
            SpriteEffects.None,
            0.1f
        );
    }

    public Component[] GetOverlayables() => new Component[] {new SimpleImage(LevelTextures.Filter[Argument-1], _position + new Vector2(7f), 9)};
}