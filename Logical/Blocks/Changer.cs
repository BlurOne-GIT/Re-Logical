using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Logical;

public class Changer : Block, IUpdateable, IOverlayable
{
    #region Fields

    private readonly BallColors _color;
    private readonly Texture2D _shadow;
    private readonly Texture2D _indicator;
    private readonly Vector2 ciPos = new Vector2(12f);
    private readonly Vector2 shadowPos = new Vector2(18f);
    #endregion

    public Changer(Point arrayPosition, byte xx, byte yy):base(arrayPosition, xx, yy)
    {
        switch (xx)
        {
            case 0x0B:
                _texture = LevelTextures.PipeHorizontal;
                _shadow = LevelTextures.ChangerShadowHorizontal;
                break;
            case 0x0C:
                _texture = LevelTextures.PipeVertical;
                _shadow = LevelTextures.ChangerShadowVertical;
                break;
            case 0x0D:
                _texture = LevelTextures.PipeCross;
                _shadow = LevelTextures.ChangerShadowCross;
                break;
            default: throw new Exception("Unhandeled");
        }
        _color = (BallColors)Argument-1;
        _indicator = LevelTextures.Indicator[Argument-1];
    }

    public void Update(GameTime gameTime)
    {
        foreach (Ball ball in Ball.allBalls)
            if (ball.Position == Statics.DetectionPoint + _position)
                ball.BallColor = _color;
    }

    public override void Render(SpriteBatch _spriteBatch)
    {
        base.Render(_spriteBatch);
        _spriteBatch.Draw(
            _indicator,
            (_position + ciPos) * Configs.Scale,
            null,
            Color.White * Statics.Opacity,
            0,
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
    
    public Component[] GetOverlayables() => new Component[] {new SimpleImage(LevelTextures.Changer[Argument-1], _position + ciPos, 9)};
}