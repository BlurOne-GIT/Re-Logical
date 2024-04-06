using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MmgEngine;

namespace Logical.Blocks;

public class Changer : Pipe, IUpdateable, IOverlayable
{
    #region Fields

    private readonly BallColors _ballColors;
    private readonly Texture2D _shadow;
    private readonly Texture2D _indicator;
    private readonly Vector2 _ciPos = new(12f);
    private readonly Vector2 _shadowPos = new(18f);
    #endregion

    public Changer(Game game, Point arrayPosition, byte xx, byte yy):base(game, arrayPosition, xx, yy, false)
    {
        _shadow = xx switch
        {
            0x0B => LevelResources.ChangerShadowHorizontal,
            0x0C => LevelResources.ChangerShadowVertical,
            0x0D => LevelResources.ChangerShadowCross,
            _ => throw new ArgumentException("Invalid Pipe direction")
        };
        _ballColors = (BallColors)Argument-1;
        _indicator = LevelResources.Indicator[Argument-1];
    }
    
    public override void Update(GameTime gameTime)
    {
        foreach (var ball in Ball.AllBalls)
            if (ball.Position == Statics.DetectionPoint + Position)
                ball.BallColor = _ballColors;
    }


    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        var spriteBatch = Game.Services.GetService<SpriteBatch>();
        spriteBatch.Draw(
            _indicator,
            (Position + _ciPos) * Configs.Scale,
            null,
            Color.White * Statics.Opacity,
            0,
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
    
    public IEnumerable<DrawableGameComponent> GetOverlayables() => new DrawableGameComponent[] {new SimpleImage(Game, LevelResources.Changer[Argument-1], Position + _ciPos, 9)};
}