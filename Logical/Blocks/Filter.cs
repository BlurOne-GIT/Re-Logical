using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MmgEngine;

namespace Logical.Blocks;

public class Filter : Pipe, IOverlayable
{
    #region Fields
    private readonly BallColors _ballColor;
    private readonly Texture2D _shadow;
    private readonly Texture2D _ball;
    private readonly Vector2 _ballPos = new(14f);
    private readonly Vector2 _shadowPos = new(10f, 11f);
    #endregion

    public Filter(Game game, Point arrayPosition, byte xx, byte yy) : base(game, arrayPosition, xx, yy, false)
    {
        _shadow = xx switch
        {
            0x05 => LevelResources.FilterShadowHorizontal,
            0x06 => LevelResources.FilterShadowVertical,
            0x07 => LevelResources.FilterShadowCross,
            _ => throw new ArgumentException("Invalid Pipe direction")
        };
        _ballColor = (BallColors)Argument-1;
        _ball = LevelResources.SpinnerBall[Argument-1];
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var ball in Ball.AllBalls.Where(ball => ball.Position == Statics.DetectionPoint + Position && ball.BallColor != _ballColor))
            ball.Bounce();
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        var spriteBatch = Game.Services.GetService<SpriteBatch>();
        spriteBatch.Draw(
            _ball,
            (Position + _ballPos) * Configs.Scale,
            null,
            Color.White * Statics.Opacity,
            0f,
            Vector2.Zero,
            Configs.Scale,
            SpriteEffects.None,
            0.1f
        );
        spriteBatch.Draw(
            _shadow,
            (Position + _shadowPos) * Configs.Scale,
            null,
            Color.White * Statics.Opacity,
            0,
            Vector2.Zero,
            Configs.Scale,
            SpriteEffects.None,
            0.1f
        );
    }

    public IEnumerable<DrawableGameComponent> GetOverlayables() => new DrawableGameComponent[] {new SimpleImage(Game, LevelResources.Filter[Argument-1], Position + new Vector2(7f), 9)};
}